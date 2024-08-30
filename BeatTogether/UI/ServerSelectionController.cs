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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using System.Threading;
using BGLib.Polyglot;
using MultiplayerCore.Models;
using MultiplayerCore.Repositories;

namespace BeatTogether.UI
{
    internal class ServerSelectionController : IInitializable, IAffinity, INotifyPropertyChanged
    {
        public const string ResourcePath = "BeatTogether.UI.ServerSelectionController.bsml";

        private FloatingScreen _screen = null!;
        private readonly MultiplayerModeSelectionFlowCoordinator _modeSelectionFlow;
        private readonly JoiningLobbyViewController _joiningLobbyView;
        private readonly NetworkConfigPatcher _networkConfig;
        private readonly MpStatusRepository _mpStatusRepository;
        private readonly ServerDetailsRegistry _serverRegistry;
        private readonly SiraLog _logger;
        private bool _isFirstActivation;
        private uint _allowSelectionOnce;

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
            MpStatusRepository mpStatusRepository,
            ServerDetailsRegistry serverRegistry,
            SiraLog logger)
        {
            _modeSelectionFlow = modeSelectionFlow;
            _joiningLobbyView = joiningLobbyView;
            _networkConfig = networkConfig;
            _mpStatusRepository = mpStatusRepository;
            _serverRegistry = serverRegistry;
            _logger = logger;
            _isFirstActivation = true;

            _serverOptions = new(_serverRegistry.Servers);
            
            _mpStatusRepository.statusUpdatedForUrlEvent += HandleMpStatusUpdateForUrl;
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

            ApplyNetworkConfig(server);
            SyncTemporarySelectedServer();
            RefreshSwitchInteractable();
            
            _modeSelectionFlow.DidDeactivate(false, false);
            _modeSelectionFlow.DidActivate(false, true, false);
            _modeSelectionFlow.ReplaceTopViewController(_joiningLobbyView,
                animationDirection: ViewController.AnimationDirection.Vertical);
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
        
        #endregion

        #region Server config
        
        private void ApplyNetworkConfig(ServerDetails server)
        {
            if (server.IsOfficial)
                _networkConfig.UseOfficialServer();
            else
                _networkConfig.UseCustomApiServer(server.ApiUrl, server.StatusUri, server.MaxPartySize,
                    null, server.DisableSsl);
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

        private void HandleMpStatusUpdateForUrl(string statusUrl, MpStatusData statusData)
        {
			// Automatically set disableSsl setting from mp status data            
			var targetServers = _serverRegistry.Servers
                .Where((server) => server.StatusUri.Equals(statusUrl));

            foreach (var targetServer in targetServers)
            {
				var disableSsl = !statusData.useSsl;

				if (disableSsl == targetServer.DisableSsl)
                    continue;
                
                _logger.Info($"Config update for \"{targetServer.ServerName}\": disableSsl={disableSsl}");
                
                targetServer.DisableSsl = disableSsl;
                
                if (_serverRegistry.SelectedServer == targetServer)
                    ApplyNetworkConfig(targetServer);
			}
        }

        #endregion

        #region Affinity patches

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator),
            nameof(MultiplayerModeSelectionFlowCoordinator.DidActivate))]
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

        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), nameof(MultiplayerModeSelectionFlowCoordinator.PresentMasterServerUnavailableErrorDialog))]
        private void PresentMasterServerUnavailableErrorDialog()
        {
            _allowSelectionOnce = 2;
		}

		[AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator),
            nameof(MultiplayerModeSelectionFlowCoordinator.DidDeactivate))]
        private void DidDeactivate(bool removedFromHierarchy)
        {
            _screen.gameObject.SetActive(false);
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator),
            nameof(MultiplayerModeSelectionFlowCoordinator.TransitionDidStart))]
        private void TransitionDidStart()
        {
            RefreshSwitchInteractable();
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator),
            nameof(MultiplayerModeSelectionFlowCoordinator.TransitionDidFinish))]
        private void TransitionDidFinish()
        {
            RefreshSwitchInteractable();
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
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator),
            nameof(MultiplayerModeSelectionFlowCoordinator.TopViewControllerWillChange))]
        private bool TopViewControllerWillChange(ViewController oldViewController, ViewController newViewController,
            ViewController.AnimationType animationType)
        {
            var screenContainer = oldViewController != null ? oldViewController.transform.parent.parent : newViewController.transform.parent.parent;
            var screenSystem = screenContainer.parent;
            
            _screen.gameObject.transform.localScale = screenContainer.localScale * screenSystem.localScale.y;
            _screen.transform.position = screenContainer.position + new Vector3(0, screenSystem.localScale.y * 1.15f, 0);
            _screen.gameObject.SetActive(true);
            
            RefreshSwitchInteractable();
            
            if (newViewController is JoiningLobbyViewController && animationType == ViewController.AnimationType.None)
                return false;
            
            return true;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(FlowCoordinator), nameof(FlowCoordinator.SetTitle))]
        private void SetTitle(ref string value, ref string ____title)
        {
            // Keep "Multiplayer Mode Selection" as a title when the server status check is happening
            // This makes it more obvious what is going on and it looks less goofy (duplicate text)
            if (value == Localization.Get("LABEL_CHECKING_SERVER_STATUS"))
                value = Localization.Get("LABEL_MULTIPLAYER_MODE_SELECTION");
        }

        #endregion

        #region SetInteraction
        
        private bool _globalInteraction = true;

        private void RefreshSwitchInteractable()
        {
            if (_serverList == null)
                return;

            // Only allow interactions when the main view controller is active and not transitioning
            var interactable = _globalInteraction
                               && _modeSelectionFlow.topViewController is MultiplayerModeSelectionViewController
                               && !_modeSelectionFlow.topViewController.isInTransition;

			// We have _allowSelectionOnce set to 2 and only enable the actual toggle
            // the second time this runs as the first will be the status check and
            // on the second time this runs we'll have the actual error pop-up
			_serverList.interactable = interactable || _allowSelectionOnce == 1;
            if (_allowSelectionOnce > 0)
                _allowSelectionOnce -= 1;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(FlowCoordinator), nameof(FlowCoordinator.SetGlobalUserInteraction))]
        private void SetGlobalUserInteraction(bool value)
        {
            _globalInteraction = value;
            RefreshSwitchInteractable();
        }
        
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}