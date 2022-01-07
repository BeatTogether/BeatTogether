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
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace BeatTogether.UI
{
    internal class ServerSelectionController : IInitializable, IAffinity
    {
        public const string ResourcePath = "BeatTogether.UI.ServerSelectionController.bsml";

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
            _screen = FloatingScreen.CreateFloatingScreen(new Vector2(75, 25), false, new Vector3(0, 1.665f, 5f), Quaternion.LookRotation(new Vector3(0, 0, 1)));
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), ResourcePath), _screen.gameObject, this);
            (_serverList.gameObject.transform.GetChild(1) as RectTransform)!.sizeDelta = new Vector2(60, 0);
            _screen.gameObject.SetActive(false);
        }

        private void ServerChanged(ServerDetails server)
        {
            _serverRegistry.SetSelectedServer(server);
            if (server.IsOfficial)
                _networkConfig.UseOfficialServer();
            else
                _networkConfig.UseMasterServer(server.EndPoint!, server.StatusUri, server.MaxPartySize);

            _mainFlow.InvokeMethod<object, FlowCoordinator>("DismissFlowCoordinator", _modeSelectionFlow, ViewController.AnimationDirection.Horizontal, null, true);
            _mainFlow.InvokeMethod<object, FlowCoordinator>("PresentFlowCoordinator", _modeSelectionFlow, null, ViewController.AnimationDirection.Horizontal, true, false);
        }

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
    }
}
