using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using IPA.Loader;
// TODO: Add some sort of loadingScreen while it checks SongPackOverrides
namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(QuickPlaySongPacksDropdown), "Start")]
    internal class QuickPlaySongPacksDropdownPatch
    {
        internal static QuickPlaySongPacksDropdown _instance;
        internal static bool didTaskFail;
        internal static void Prefix(QuickPlaySongPacksDropdown __instance)
        {
            _instance = __instance;
        }

        public static void UpdateSongPacks()
        {
            _instance.SetField<QuickPlaySongPacksDropdown, MasterServerQuickPlaySetupData.QuickPlaySongPacksOverride>("_quickPlaySongPacksOverride", null);
            var pluginMetadata = PluginManager.GetPluginFromId("MultiplayerExtensions");
            if ((pluginMetadata == null || !PluginManager.IsEnabled(pluginMetadata)) && !Plugin.ServerDetailProvider.Selection.IsOfficial)
            {
                Plugin.Logger.Info("MultiplayerExtensions not installed, not overriding packs");
                return;
            }

            System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    Plugin.Logger.Info("Get QuickPlaySongPacksOverride");
                    var quickPlaySetupData = await MasterServerQuickPlaySetupModelInitPatch.instance.GetQuickPlaySetupAsync(System.Threading.CancellationToken.None);
                    didTaskFail = false;
                    return quickPlaySetupData.quickPlayAvailablePacksOverride;
                }
                catch (Exception)
                {
                    Plugin.Logger.Info("Could not get QuickPlaySongPacksOverride");
                    didTaskFail = true;
                    return null;
                }
            }).ContinueWith(r =>
            {
                Plugin.Logger.Debug("ContinueWith running for quickplaySongPackOverrides");
                _instance.SetField("_quickPlaySongPacksOverride", r.Result);
                _instance.SetField("_initialized", false);
                //_instance.LazyInit();
            }
            );
        }
    }

    [HarmonyPatch(typeof(QuickPlaySongPacksDropdown), "LazyInit")]
    class QuickPlaySongPacksDropdownLazyInitPatch
    {
        internal static void Prefix(QuickPlaySongPacksDropdown __instance, ref MasterServerQuickPlaySetupData.QuickPlaySongPacksOverride ____quickPlaySongPacksOverride, ref bool ____initialized)
        {
            var pluginMetadata = PluginManager.GetPluginFromId("MultiplayerExtensions");
            if ((pluginMetadata == null || !PluginManager.IsEnabled(pluginMetadata)) && !Plugin.ServerDetailProvider.Selection.IsOfficial)
            {
                Plugin.Logger.Info("MultiplayerExtensions not installed, not overriding packs");
                ____quickPlaySongPacksOverride = null;
                ____initialized = false;
            }
            else if (QuickPlaySongPacksDropdownPatch.didTaskFail)
            {
                ____quickPlaySongPacksOverride = null;
                ____initialized = false;
            }
        }
    }

}
