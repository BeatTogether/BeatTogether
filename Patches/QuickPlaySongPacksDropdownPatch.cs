using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using BeatTogether.Providers;
using System.Threading;
// TODO: Add some sort of loadingScreen while it checks SongPackOverrides
namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(QuickPlaySongPacksDropdown), "Start")]
    internal class QuickPlaySongPacksDropdownPatch
    {
        public enum TaskStateEnum
        {
            Unknown, Finished, Running, Failed
        }
        public static TaskStateEnum TaskState { get; private set; } = TaskStateEnum.Unknown;
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        internal static void Prefix(QuickPlaySongPacksDropdown __instance)
        {
            GameClassInstanceProvider.Instance.QuickPlaySongPacksDropdown = __instance;
        }

        public static void UpdateSongPacks()
        {
            if (cancellationTokenSource.Token.CanBeCanceled) cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            QuickPlaySongPacksDropdown instance = GameClassInstanceProvider.Instance.QuickPlaySongPacksDropdown;
            if (instance != null)
                instance.SetField<QuickPlaySongPacksDropdown, MasterServerQuickPlaySetupData.QuickPlaySongPacksOverride>("_quickPlaySongPacksOverride", null);
            if (ModStatusProvider.ShouldBlockSongPackOverrides)
            {
                Plugin.Logger.Info("MultiplayerExtensions not installed, not overriding packs");
                return;
            }
            TaskState = TaskStateEnum.Running;
            System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    Plugin.Logger.Info("Get QuickPlaySongPacksOverride");
                    var quickPlaySetupData = await GameClassInstanceProvider.Instance.MasterServerQuickPlaySetupModel.GetQuickPlaySetupAsync(cancellationTokenSource.Token);
                    return quickPlaySetupData.quickPlayAvailablePacksOverride;
                }
                catch (Exception)
                {
                    Plugin.Logger.Warn("Could not get QuickPlaySongPacksOverride");
                    TaskState = TaskStateEnum.Failed;
                    return null;
                }
            }).ContinueWith(r =>
            {
                Plugin.Logger.Debug("ContinueWith running for quickplaySongPackOverrides");
                if (instance != null)
                {
                    instance.SetField("_quickPlaySongPacksOverride", r.Result);
                    instance.SetField("_initialized", false);
                }
                if (TaskState == TaskStateEnum.Running) TaskState = TaskStateEnum.Finished;
                //MultiplayerModeSelectionFlowCoordinatorPatch.ShowQuickPlayLobbyScreenIfWaiting();
                //GameClassInstanceProvider.Instance.QuickPlaySongPacksDropdown.LazyInit();
            }
            );
        }
    }

    [HarmonyPatch(typeof(QuickPlaySongPacksDropdown), "LazyInit")]
    class QuickPlaySongPacksDropdownLazyInitPatch
    {
        internal static void Prefix(QuickPlaySongPacksDropdown __instance, ref MasterServerQuickPlaySetupData.QuickPlaySongPacksOverride ____quickPlaySongPacksOverride, ref bool ____initialized)
        {
            if (ModStatusProvider.ShouldBlockSongPackOverrides || QuickPlaySongPacksDropdownPatch.TaskState != QuickPlaySongPacksDropdownPatch.TaskStateEnum.Finished)
            {
                ____quickPlaySongPacksOverride = null;
                ____initialized = false;
            }
        }
    }

}
