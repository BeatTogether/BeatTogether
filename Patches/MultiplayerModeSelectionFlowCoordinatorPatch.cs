using BeatTogether.Providers;
using HarmonyLib;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QPSPDP = BeatTogether.Patches.QuickPlaySongPacksDropdownPatch;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "HandleJoinQuickPlayViewControllerDidFinish")]
    class MultiplayerModeSelectionFlowCoordinatorPatch
    {
        internal static MultiplayerModeSelectionFlowCoordinator instance;
        internal static JoinQuickPlayViewController joinQuickPlayViewController;
        internal static JoiningLobbyViewController joiningLobbyViewController;
        internal static bool isWaitingForPacks = false;
        internal static bool Prefix(MultiplayerModeSelectionFlowCoordinator __instance, bool success, JoiningLobbyViewController ____joiningLobbyViewController, JoinQuickPlayViewController ____joinQuickPlayViewController)
        {
            instance = __instance;
            joinQuickPlayViewController = ____joinQuickPlayViewController;
            joiningLobbyViewController = ____joiningLobbyViewController;
            if (success && (QPSPDP.TaskState != QPSPDP.TaskStateEnum.Finished || QPSPDP.TaskState != QPSPDP.TaskStateEnum.Finished))
            {
                ____joiningLobbyViewController.Init("Loading Quickplay Pack Overrides");
                ____joiningLobbyViewController.didCancelEvent += OnDidCancelEvent;
                __instance.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("ReplaceTopViewController", new object[] {
                                    ____joiningLobbyViewController, null, ViewController.AnimationType.In, ViewController.AnimationDirection.Vertical});
                isWaitingForPacks = true;
                return false;
            } else
            {
                isWaitingForPacks = false;
                return true;
            }
        }

        internal static void OnDidCancelEvent()
        {
            instance.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("ReplaceTopViewController", new object[] {
                                    GameClassInstanceProvider.Instance.MultiplayerModeSelectionViewController, null, ViewController.AnimationType.In, ViewController.AnimationDirection.Vertical});

            joiningLobbyViewController.didCancelEvent -= OnDidCancelEvent;
        }

        public static void ShowQuickPlayLobbyScreenIfWaiting()
        {
            if (isWaitingForPacks && instance != null && joinQuickPlayViewController != null)
            {
                instance.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("ReplaceTopViewController", new object[] {
                                    joinQuickPlayViewController, null, ViewController.AnimationType.In, ViewController.AnimationDirection.Vertical});
            }
        }

    }
}
