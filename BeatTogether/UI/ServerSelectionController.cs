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
using IPA.Config.Stores.Attributes;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

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

        private FloatingScreen _screen = null!;

        private readonly MultiplayerModeSelectionFlowCoordinator _modeSelectionFlow;
        private readonly JoiningLobbyViewController _joiningLobbyView;
        private readonly NetworkConfigPatcher _networkConfig;
        private readonly ServerDetailsRegistry _serverRegistry;
        private readonly SiraLog _logger;

        [UIComponent("server-list")] private ListSetting _serverList = null!;

        [UIValue("server")]
        private ServerDetails _serverValue
        {
            get => _serverRegistry.SelectedServer;
            set => ServerChanged(value);
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

        private void ServerChanged(ServerDetails server)
        {
            if (server is TemporaryServerDetails)
                return;
            
            _logger.Debug($"Server changed to '{server.ServerName}': '{server.HostName}:{server.Port}'");
            _serverRegistry.SetSelectedServer(server);
            if (server.IsOfficial)
                _networkConfig.UseOfficialServer();
            else
                _networkConfig.UseMasterServer(server.EndPoint!, server.StatusUri, server.MaxPartySize);

            SyncTemporarySelectedServer();
            
            _serverList.interactable = false;
            _didDeactivate(_modeSelectionFlow, false, false);
            _didActivate(_modeSelectionFlow, false, true, false);
            _replaceTopScreenViewController(_modeSelectionFlow, _joiningLobbyView, HandleTransitionFinished,
                ViewController.AnimationType.None, ViewController.AnimationDirection.Vertical);
        }

        private void HandleTransitionFinished()
        {
        }
        //=> _serverList.interactable = true;

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidActivate")]
        private void DidActivate()
        {
            SyncSelectedServer();
        }

        private void SyncSelectedServer()
        {
            var didChangeSelection = false;

            if (_networkConfig.MasterServerEndPoint is not null)
            {
                // Master server is being patched by MpCore, sync our selection
                var knownServer = _serverRegistry.Servers.FirstOrDefault(
                    sd => sd.EndPoint?.Equals(_networkConfig.MasterServerEndPoint) ?? false
                );

                if (knownServer != _serverRegistry.SelectedServer)
                {
                    _logger.Info("Changing server selection from patch sync!");
                    _serverRegistry.SetSelectedServer(knownServer
                                                      ?? new TemporaryServerDetails(_networkConfig.MasterServerEndPoint));
                    didChangeSelection = true;
                }
            }
            else
            {
                // No one is patching the master server, restore our selected server from config
                if (_serverRegistry.SelectedServer.IsOfficial)
                    _networkConfig.UseOfficialServer();
                else
                    _networkConfig.UseMasterServer(_serverRegistry.SelectedServer.EndPoint!,
                        _serverRegistry.SelectedServer.StatusUri, _serverRegistry.SelectedServer.MaxPartySize);
            }
            
            // Sync UI
            if (didChangeSelection)
            {
                SyncTemporarySelectedServer();
                OnPropertyChanged(nameof(_serverValue)); // for BSML binding
            }

            _screen.gameObject.SetActive(true);
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

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "DidDeactivate")]
        private void DidDeactivate(bool removedFromHierarchy)
        {
            _screen.gameObject.SetActive(false);
            if (removedFromHierarchy)
                _networkConfig.UseOfficialServer();
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerModeSelectionFlowCoordinator), "TopViewControllerWillChange")]
        private bool TopViewControllerWillChange(ViewController oldViewController, ViewController newViewController,
            ViewController.AnimationType animationType)
        {
            if (newViewController is JoiningLobbyViewController)
                _serverList.interactable = oldViewController is MultiplayerModeSelectionViewController;
            if (newViewController is MultiplayerModeSelectionViewController &&
                oldViewController is JoiningLobbyViewController)
                _serverList.interactable = true;
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
        [AffinityPatch(typeof(FlowCoordinator), "SetTitle")]
        private void SetTitle(ref string value)
        {
            if (value == Localization.Get("LABEL_CHECKING_SERVER_STATUS"))
                value = Localization.Get("LABEL_MULTIPLAYER_MODE_SELECTION");
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(FlowCoordinator), "SetGlobalUserInteraction")]
        private void SetGlobalUserInteraction(bool value)
            => _serverList.interactable = value;

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