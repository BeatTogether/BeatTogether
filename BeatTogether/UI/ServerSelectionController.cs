using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatTogether.Models;
using BeatTogether.Registries;
using HMUI;
using IPA.Utilities;
using MultiplayerCore.Patchers;
using Polyglot;
using SiraUtil.Affinity;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using System.Threading;

namespace BeatTogether.UI
{
    internal class ServerSelectionController : IInitializable, IAffinity, INotifyPropertyChanged
    {
        public const string ResourcePath = "BeatTogether.UI.ServerSelectionController.bsml";

        private Action<FlowCoordinator, bool, bool, bool> _didActivate
            = MethodAccessor<FlowCoordinator, Action<FlowCoordinator, bool, bool, bool>>
                .GetDelegate("DidActivate");

        private Action<FlowCoordinator, bool, bool> _didDeactivate
            = MethodAccessor<FlowCoordinator, Action<FlowCoordinator, bool, bool>>
                .GetDelegate("DidDeactivate");

        private Action<FlowCoordinator, ViewController, Action, ViewController.AnimationType,
            ViewController.AnimationDirection> _replaceTopScreenViewController
            = MethodAccessor<FlowCoordinator, Action<FlowCoordinator, ViewController, Action,
                    ViewController.AnimationType, ViewController.AnimationDirection>>
                .GetDelegate("ReplaceTopViewController");

        private FieldAccessor<MultiplayerModeSelectionFlowCoordinator, CancellationTokenSource>.Accessor _cancellationTokenSource
            = FieldAccessor<MultiplayerModeSelectionFlowCoordinator, CancellationTokenSource>.GetAccessor(nameof(_cancellationTokenSource));

        private FloatingScreen _screen = null!;

        private MultiplayerModeSelectionFlowCoordinator _modeSelectionFlow;
        private readonly JoiningLobbyViewController _joiningLobbyView;
        private readonly NetworkConfigPatcher _networkConfig;
        private readonly ServerDetailsRegistry _serverRegistry;
        private readonly SiraLog _logger;
        private bool _isFirstActivation;

        [UIComponent("server-list")] private ListSetting _serverList = null!;

        [UIValue("server")]
        private ServerDetails _serverValue
        {
            get => _serverRegistry.SelectedServer;
            set => ApplySelectedServer(value);
        }

        [UIValue("server-options")] private List<object> _serverOptions;

        internal ServerSelectionController(
            MultiplayerModeSelectionFlowCoordinator modeSelectionFlow,
            JoiningLobbyViewController joiningLobbyView,
            NetworkConfigPatcher networkConfig,
            ServerDetailsRegistry serverRegistry,
            SiraLog logger)
        {
            _modeSelectionFlow = modeSelectionFlow;
            _joiningLobbyView = joiningLobbyView;
            _networkConfig = networkConfig;
            _serverRegistry = serverRegistry;
            _logger = logger;
            _isFirstActivation = true;

            _serverOptions = new(_serverRegistry.Servers);
        }

        public void Initialize()
        {
            _screen = FloatingScreen.CreateFloatingScreen(new Vector2(90, 90), false, new Vector3(0, 3f, 4.35f),
                new Quaternion(0, 0, 0, 0));
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), ResourcePath),
                _screen.gameObject, this);
            (_serverList.gameObject.transform.GetChild(1) as RectTransform)!.sizeDelta = new Vector2(60, 0);
            _screen.GetComponent<CurvedCanvasSettings>().SetRadius(140);
            _screen.gameObject.SetActive(false);
        }

        #region Server selection

        private void ApplySelectedServer(ServerDetails server)
        {
            if (server is TemporaryServerDetails)
                return;
            
            _logger.Debug($"Server changed to '{server.ServerName}': '{server.ApiUrl}'");
            
            _serverRegistry.SetSelectedServer(server);
            ApplyNetworkConfig(server);
            SyncTemporarySelectedServer();

            SetInteraction(false);
            //_cancellationTokenSource(ref _modeSelectionFlow) = new CancellationTokenSource();
            _didDeactivate(_modeSelectionFlow, false, false);
            _didActivate(_modeSelectionFlow, false, true, false);
            _replaceTopScreenViewController(_modeSelectionFlow, _joiningLobbyView, () => { },
                ViewController.AnimationType.None, ViewController.AnimationDirection.Vertical);
        }

        private void ApplyNetworkConfig(ServerDetails server)
        {
            if (server.IsOfficial)
                _networkConfig.UseOfficialServer();
            else
                _networkConfig.UseCustomApiServer(server.ApiUrl, server.StatusUri, server.MaxPartySize,
                    null, server.DisableSsl);
        }
        
        private void SyncSelectedServer()
        {
            ServerDetails selectedServer;
            
            if (_networkConfig.IsOverridingApi)
            {
                // Master server is being patched by MpCore, sync our selection
                var knownServer = _serverRegistry.Servers.FirstOrDefault(serverDetails =>
                    serverDetails.MatchesApiUrl(_networkConfig.GraphUrl));

                if (knownServer != null)
                {
                    // Selected server is in our config
                    selectedServer = knownServer;
                }
                else
                {
                    // Selected server is not in our config, set temporary value
                    _logger.Debug($"Setting temporary server details (GraphUrl={_networkConfig.GraphUrl})");
                    selectedServer = new TemporaryServerDetails(_networkConfig.GraphUrl!, _networkConfig.MasterServerStatusUrl);
                }
            }
            else
            {
                selectedServer = _serverRegistry.OfficialServer;
            }

            _serverRegistry.SetSelectedServer(selectedServer);
            
            SyncTemporarySelectedServer();
            
            OnPropertyChanged(nameof(_serverValue)); // for BSML binding
        }

        private void SyncTemporarySelectedServer()
        {
            var didChange = false;
            
            if (_serverRegistry.TemporarySelectedServer is not null)
            {
                var temporaryServer = _serverRegistry.TemporarySelectedServer!;

                if (!_serverOptions.Contains(temporaryServer))
                {
                    _serverOptions.Add(temporaryServer);
                    didChange = true;
                }
            }
            else
            {
                if (_serverOptions.RemoveAll(so => so is TemporaryServerDetails) > 0)
                    didChange = true;
            }

            if (didChange)
                OnPropertyChanged(nameof(_serverOptions)); // for BSML binding
        }
        
        #endregion

        #region Affinity patches

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidActivate")]
        private void DidActivate()
        {
            if (_isFirstActivation)
            {
                // First activation: apply the currently selected server (from our config)
                ApplyNetworkConfig(_serverRegistry.SelectedServer);
                _isFirstActivation = false;
            }
            else
            {
                // Secondary activation: server selection may have been externally modified, sync it now
                SyncSelectedServer();
                _screen.gameObject.SetActive(true);
            }
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidDeactivate")]
        private void DidDeactivate(bool removedFromHierarchy)
        {
            _screen.gameObject.SetActive(false);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "TopViewControllerWillChange")]
        private bool TopViewControllerWillChange(ViewController oldViewController, ViewController newViewController,
            ViewController.AnimationType animationType)
        {
            Transform screenContainer = oldViewController != null ? oldViewController.transform.parent.parent : newViewController.transform.parent.parent;
            Transform screenSystem = screenContainer.parent;
            _screen.gameObject.transform.localScale = screenContainer.localScale * screenSystem.localScale.y;
            _screen.transform.position = screenContainer.position + new Vector3(0, screenSystem.localScale.y * 1.15f, 0);
            _screen.gameObject.SetActive(true);

            if (oldViewController is MultiplayerModeSelectionViewController)
                SetInteraction(false);
            if (newViewController is MultiplayerModeSelectionViewController || newViewController is ConnectionErrorDialogViewController)
                SetInteraction(true);
            if (newViewController is JoiningLobbyViewController && animationType == ViewController.AnimationType.None)
                return false;
            return true;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(ViewControllerTransitionHelpers),
            nameof(ViewControllerTransitionHelpers.DoPresentTransition))]
        private void DoPresentTransition(ViewController toPresentViewController, ViewController toDismissViewController,
            ref ViewController.AnimationDirection animationDirection)
        {
            if (toDismissViewController is JoiningLobbyViewController)
                animationDirection = ViewController.AnimationDirection.Vertical;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerUnavailableReasonMethods),
                       nameof(MultiplayerUnavailableReasonMethods.TryGetMultiplayerUnavailableReason))]
        private void TryGetMultiplayerUnavailableReason()
        {
            // Re-Enable interaction when the MultiplayerUnavailableReason method is called, which happens when the initial status check returns an error
            if (_serverList.enabled)
                SetInteraction(true);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(FlowCoordinator), "SetTitle")]
        private void SetTitle(ref string value, ref string ____title)
        {
            if (value == Localization.Get("LABEL_CHECKING_SERVER_STATUS"))
                value = Localization.Get("LABEL_MULTIPLAYER_MODE_SELECTION");
            if (____title == Localization.Get("LABEL_CHECKING_SERVER_STATUS") && value == "")
                SetInteraction(true);
        }
        
        #endregion

        #region SetInteraction
        
        private bool _interactable = true;
        private bool _globalInteraction = true;

        private void SetInteraction(bool value)
        {
            _interactable = value;
            _serverList.interactable = _interactable && _globalInteraction;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(FlowCoordinator), "SetGlobalUserInteraction")]
        private void SetGlobalUserInteraction(bool value)
        {
            _globalInteraction = value;
            _serverList.interactable = _interactable && _globalInteraction;
        }
        
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}