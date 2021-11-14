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
    [HarmonyPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidActivate")]
    class MultiplayerModeSelectionFlowCoordinatorDidActivatePatch
    {
        internal static void Prefix(MultiplayerModeSelectionFlowCoordinator __instance, JoiningLobbyViewController ____joiningLobbyViewController, JoinQuickPlayViewController ____joinQuickPlayViewController)
        {
            GameClassInstanceProvider classInstanceProvider = GameClassInstanceProvider.Instance;
            classInstanceProvider.MultiplayerModeSelectionFlowCoordinator = __instance;
            classInstanceProvider.JoinQuickPlayViewController = ____joinQuickPlayViewController;
            classInstanceProvider.JoiningLobbyViewController = ____joiningLobbyViewController;
        }

    }

    //[HarmonyPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "HandleMultiplayerLobbyControllerDidFinish")]
    //class MultiplayerModeSelectionFlowCoordinatorPatch
    //{
    //    internal static bool isWaitingForPacks = false;
    //    internal static bool Prefix(MultiplayerModeSelectionFlowCoordinator __instance, MultiplayerModeSelectionViewController viewController, MultiplayerModeSelectionViewController.MenuButton menuButton, JoiningLobbyViewController ____joiningLobbyViewController, JoinQuickPlayViewController ____joinQuickPlayViewController)
    //    {
    //        if (menuButton == MultiplayerModeSelectionViewController.MenuButton.QuickPlay && QPSPDP.TaskState == QPSPDP.TaskStateEnum.Running)
    //        {
    //            ____joiningLobbyViewController.Init("Loading Quickplay Pack Overrides");
    //            ____joiningLobbyViewController.didCancelEvent += OnDidCancelEvent;
    //            //__instance.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("ReplaceTopViewController", new object[] {
    //            //                    ____joiningLobbyViewController, null, ViewController.AnimationType.In, ViewController.AnimationDirection.Vertical});
    //            __instance.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("PresentViewController", new object[] {
    //                                ____joiningLobbyViewController, null, ViewController.AnimationDirection.Vertical, false});
    //            isWaitingForPacks = true;
    //            ShowQuickPlayLobbyScreenIfWaiting();
    //            return false;
    //        } else
    //        {
    //            isWaitingForPacks = false;
    //            return true;
    //        }
    //    }

    //    internal static void OnDidCancelEvent()
    //    {
    //        GameClassInstanceProvider classInstanceProvider = GameClassInstanceProvider.Instance;
    //        //classInstanceProvider.JoinQuickPlayViewController.gameObject.SetActive(true);
    //        //classInstanceProvider.MultiplayerModeSelectionFlowCoordinator.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("DismissViewController", new object[] {
    //        //                        classInstanceProvider.JoiningLobbyViewController, ViewController.AnimationDirection.Vertical, null, false});
    //        classInstanceProvider.MultiplayerModeSelectionFlowCoordinator.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("PresentViewController", new object[] {
    //                                classInstanceProvider.MultiplayerModeSelectionViewController, null, ViewController.AnimationDirection.Vertical, false});
    //        //classInstanceProvider.MultiplayerModeSelectionFlowCoordinator.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("ReplaceTopViewController", new object[] {
    //        //                        classInstanceProvider.MultiplayerModeSelectionViewController, null, ViewController.AnimationType.In, ViewController.AnimationDirection.Vertical});


    //        classInstanceProvider.JoiningLobbyViewController.didCancelEvent -= OnDidCancelEvent;
    //    }

    //    public static void ShowQuickPlayLobbyScreenIfWaiting()
    //    {
    //        GameClassInstanceProvider classInstanceProvider = GameClassInstanceProvider.Instance;
    //        MultiplayerModeSelectionFlowCoordinator instance = classInstanceProvider.MultiplayerModeSelectionFlowCoordinator;
    //        if (isWaitingForPacks && instance != null && classInstanceProvider.JoinQuickPlayViewController != null && QPSPDP.TaskState != QPSPDP.TaskStateEnum.Running)
    //        {
    //            classInstanceProvider.JoinQuickPlayViewController.Setup(
    //                ReflectionUtil.GetField<MasterServerQuickPlaySetupData, MultiplayerModeSelectionFlowCoordinator>(instance, "_masterServerQuickPlaySetupData"), 
    //                ReflectionUtil.GetField<PlayerDataModel, MultiplayerModeSelectionFlowCoordinator>(instance, "_playerDataModel").playerData.multiplayerModeSettings);
    //            instance.InvokeMethod<object, MultiplayerModeSelectionFlowCoordinator>("PresentViewController", new object[] {
    //                                classInstanceProvider.JoinQuickPlayViewController,  null, ViewController.AnimationDirection.Vertical, false});
    //        }
    //        else
    //        {
    //            Plugin.Logger.Debug($"isWaitingForPacks: {(isWaitingForPacks ? "true" : "false")}, QPSPDP.TaskState: {(QPSPDP.TaskState)}");
    //        }
    //    }

    //}
}
