using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Loader;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MasterServerQuickPlaySetupModel), "Init")]
    class MasterServerQuickPlaySetupModelInitPatch
    {
        public static MasterServerQuickPlaySetupModel instance { get; private set; }
        internal static void Postfix(MasterServerQuickPlaySetupModel __instance)
        {
            instance = __instance;
        }
    }

    // TODO: Should probably check if the server was switched or not, for now we just always return false
    [HarmonyPatch(typeof(MasterServerQuickPlaySetupModel), "IsQuickPlaySetupTaskValid")]
    class IsQuickPlaySetupTaskValidPatch
    {
        internal static void Postfix(MasterServerQuickPlaySetupModel __instance, ref bool __result, ref Task<MasterServerQuickPlaySetupData> ____request)
        {
            ____request = null;
            __result = false;
        }
    }

    [HarmonyPatch(typeof(MasterServerQuickPlaySetupModel), "GetQuickPlaySetupAsync")]
    class GetQuickPlaySetupAsyncPatch
    {
        internal static void Postfix(MasterServerQuickPlaySetupModel __instance, ref Task<MasterServerQuickPlaySetupData> __result, ref Task<MasterServerQuickPlaySetupData> ____request)
        {
            var pluginMetadata = PluginManager.GetPluginFromId("MultiplayerExtensions");
            if ((pluginMetadata == null || !PluginManager.IsEnabled(pluginMetadata)) && !Plugin.ServerDetailProvider.Selection.IsOfficial)
            {
                Plugin.Logger.Info("MultiplayerExtensions not installed, returning 'null'");
                __result = new Task<MasterServerQuickPlaySetupData>(null);
            }

        }
    }

}
