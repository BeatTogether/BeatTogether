using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatTogether.Models;
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
        public const string ResourcePath = "BeatTogether.UI.ServerSelection.bsml";
        private ServerDetail _selectedServer;

        private FieldAccessor<HierarchyManager, ScreenSystem>.Accessor _screenSystemAccessor
            = FieldAccessor<HierarchyManager, ScreenSystem>.GetAccessor("_screenSystem");

        private ScreenSystem _screenSystem
            => _screenSystemAccessor(ref _hierarchyManager);

        private HierarchyManager _hierarchyManager;
        private readonly MainFlowCoordinator _mainFlow;
        private readonly MultiplayerModeSelectionFlowCoordinator _modeSelectionFlow;
        private readonly NetworkConfigPatcher _networkConfigPatcher;
        private readonly Config _config;
        private readonly SiraLog _logger;

        [UIComponent("server-list")]
        private ListSetting _serverList = null!;

        [UIValue("server")]
        private ServerDetail _serverValue
        {
            get => _selectedServer;
            set {
                _selectedServer = value;
                if (_selectedServer.IsOfficial)
                    _networkConfigPatcher.UseOfficialConfig();
                else
                    _networkConfigPatcher.UseMasterServer(_selectedServer.EndPoint!, _selectedServer.StatusUri, _selectedServer.MaxPartySize);

                _mainFlow.InvokeMethod<object, FlowCoordinator>("DismissFlowCoordinator", _modeSelectionFlow, ViewController.AnimationDirection.Horizontal, null, true);
                _mainFlow.InvokeMethod<object, FlowCoordinator>("PresentFlowCoordinator", _modeSelectionFlow, null, ViewController.AnimationDirection.Horizontal, true, false);
            }
        }

        [UIValue("server-options")]
        private List<object> _serverOptions;

        internal ServerSelectionController(
            HierarchyManager hierarchyManager,
            MainFlowCoordinator mainFlow,
            MultiplayerModeSelectionFlowCoordinator modeSelectionFlow,
            NetworkConfigPatcher networkConfigPatcher,
            Config config,
            SiraLog logger)
        {
            _hierarchyManager = hierarchyManager;
            _mainFlow = mainFlow;
            _modeSelectionFlow = modeSelectionFlow;
            _networkConfigPatcher = networkConfigPatcher;
            _config = config;
            _logger = logger;

            List<ServerDetail> servers = new(_config.Servers);
            servers.Add(new ServerDetail
            {
                ServerName = Config.OfficialServerName
            });

            _selectedServer = servers.Find(server => server.ServerName == _config.SelectedServer);
            _serverOptions = new(servers);
        }

        public void Initialize()
        {
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), ResourcePath), _screenSystem.titleViewController.gameObject, this);
            (_serverList.gameObject.transform.GetChild(1) as RectTransform)!.sizeDelta = new Vector2(60, 0);
            _serverList.transform.position += new Vector3(0, -0.5f, 0);
            _serverList.gameObject.SetActive(false);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidActivate")]
        private void DidActivate()
        {
            if (_selectedServer.IsOfficial)
                _networkConfigPatcher.UseOfficialConfig();
            else
                _networkConfigPatcher.UseMasterServer(_selectedServer.EndPoint!, _selectedServer.StatusUri, _selectedServer.MaxPartySize);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidDeactivate")]
        private void DidDeactivate() => _serverList.gameObject.SetActive(false);

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), nameof(MultiplayerModeSelectionFlowCoordinator.TryShowModeSelection))]
        private void TryShowModeSelection() => _serverList.gameObject.SetActive(true);

        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "TopViewControllerWillChange")]
        private void TopViewControllerWillChange(ViewController newViewController)
        {
            _serverList.gameObject.SetActive(
                newViewController is MultiplayerModeSelectionViewController ||
                newViewController is SimpleDialogPromptViewController);
        }
    }
}
