using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatTogether.Models;
using BeatTogether.Registries;
using HMUI;
using IPA.Utilities;
using MultiplayerCore.Patchers;
using SiraUtil.Affinity;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace BeatTogether.UI
{
    internal class ServerSelectionController : IInitializable, IAffinity
    {
        public const string ResourcePath = "BeatTogether.UI.ServerSelectionController.bsml";

        private Action<FlowCoordinator, FlowCoordinator, ViewController.AnimationDirection, Action, bool> _dismissFlowCoordinator
            = MethodAccessor<FlowCoordinator, Action<FlowCoordinator, FlowCoordinator, ViewController.AnimationDirection, Action, bool>>
                .GetDelegate("DismissFlowCoordinator");
        private Action<FlowCoordinator, FlowCoordinator, Action, ViewController.AnimationDirection, bool, bool> _presentFlowCoordinator
            = MethodAccessor<FlowCoordinator, Action<FlowCoordinator, FlowCoordinator, Action, ViewController.AnimationDirection, bool, bool>>
                .GetDelegate("PresentFlowCoordinator");

        private FloatingScreen _screen = null!;

        private readonly MainFlowCoordinator _mainFlow;
        private readonly MultiplayerModeSelectionFlowCoordinator _modeSelectionFlow;
        private readonly NetworkConfigPatcher _networkConfig;
        private readonly ServerDetailsRegistry _serverRegistry;
        private readonly SiraLog _logger;

        [UIComponent("server-list")]
        private ListSetting _serverList = null!;

        [UIValue("server")]
        private ServerDetails _serverValue
        {
            get => _serverRegistry.SelectedServer;
            set => ServerChanged(value);
        }

        [UIValue("server-options")]
        private List<object> _serverOptions;

        internal ServerSelectionController(
            MainFlowCoordinator mainFlow,
            MultiplayerModeSelectionFlowCoordinator modeSelectionFlow,
            NetworkConfigPatcher networkConfig,
            ServerDetailsRegistry serverRegistry,
            SiraLog logger)
        {
            _mainFlow = mainFlow;
            _modeSelectionFlow = modeSelectionFlow;
            _networkConfig = networkConfig;
            _serverRegistry = serverRegistry;
            _logger = logger;

            _serverOptions = new(_serverRegistry.Servers);
        }

        public void Initialize()
        {
            _screen = FloatingScreen.CreateFloatingScreen(new Vector2(90, 90), false, new Vector3(0, 2.4f, 4.35f), new Quaternion(0,0,0,0));
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), ResourcePath), _screen.gameObject, this);
            (_serverList.gameObject.transform.GetChild(1) as RectTransform)!.sizeDelta = new Vector2(60, 0);
            _screen.GetComponent<CurvedCanvasSettings>().SetRadius(140);
            _screen.gameObject.SetActive(false);
        }

        private void ServerChanged(ServerDetails server)
        {
            _logger.Debug($"Server changed to '{server.ServerName}': '{server.HostName}:{server.Port}'");
            _serverRegistry.SetSelectedServer(server);
            if (server.IsOfficial)
                _networkConfig.UseOfficialServer();
            else
                _networkConfig.UseMasterServer(server.EndPoint!, server.StatusUri, server.MaxPartySize);

            _serverList.interactable = false;
            _dismissFlowCoordinator(_mainFlow, _modeSelectionFlow, ViewController.AnimationDirection.Horizontal, null!, true);
            _presentFlowCoordinator(_mainFlow, _modeSelectionFlow, HandleTransitionFinished, ViewController.AnimationDirection.Horizontal, true, false);
        }

        private void HandleTransitionFinished()
            => _serverList.interactable = true;

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidActivate")]
        private void DidActivate()
        {
            if (_serverRegistry.SelectedServer.IsOfficial)
                _networkConfig.UseOfficialServer();
            else
                _networkConfig.UseMasterServer(_serverRegistry.SelectedServer.EndPoint!, _serverRegistry.SelectedServer.StatusUri, _serverRegistry.SelectedServer.MaxPartySize);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidDeactivate")]
        private void DidDeactivate()
        {
            _screen.gameObject.SetActive(false);
            _networkConfig.UseOfficialServer();
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), nameof(MultiplayerModeSelectionFlowCoordinator.TryShowModeSelection))]
        private void TryShowModeSelection() 
            => _screen.gameObject.SetActive(true);

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "TopViewControllerWillChange")]
        private void TopViewControllerWillChange(ViewController oldViewController, ViewController newViewController)
        {
            if (newViewController is JoiningLobbyViewController)
                _serverList.interactable = oldViewController is MultiplayerModeSelectionViewController;
            if (newViewController is MultiplayerModeSelectionViewController && oldViewController is JoiningLobbyViewController)
                _serverList.interactable = true;
        }
    }
}
