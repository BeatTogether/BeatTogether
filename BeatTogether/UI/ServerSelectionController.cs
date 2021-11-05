using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatTogether.Models;
using BeatTogether.Providers;
using IPA.Utilities;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Zenject;

namespace BeatTogether.UI
{
    [HotReload(RelativePathToLayout = @"..\UI\ServerSelectionController.bsml")]
    [ViewDefinition("BeatTogether.UI.ServerSelectionController.bsml")]
    internal class ServerSelectionController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        private ServerDetailProvider _serverDetails = null!;
        private IUnifiedNetworkPlayerModel _networkPlayerModel = null!;
        private IMasterServerAvailabilityModel _serverAvailabilityModel = null!;
        private IMasterServerQuickPlaySetupModel _serverQuickPlaySetupModel = null!;
        private MultiplayerModeSelectionViewController _multiplayerModeSelectionView = null!;
        private MultiplayerModeSelectionFlowCoordinator _multiplayerModeSelectionFlow = null!;

        private FieldAccessor<MultiplayerModeSelectionViewController, TextMeshProUGUI>.Accessor _maintenanceMessageTextAccessor
            = FieldAccessor<MultiplayerModeSelectionViewController, TextMeshProUGUI>.GetAccessor("_maintenanceMessageText");
        private FieldAccessor<MultiplayerModeSelectionFlowCoordinator, MasterServerQuickPlaySetupData>.Accessor _quickPlaySetupDataAccessor
            = FieldAccessor<MultiplayerModeSelectionFlowCoordinator, MasterServerQuickPlaySetupData>.GetAccessor("_masterServerQuickPlaySetupData");
        
        private TextMeshProUGUI _maintenanceMessageText
        {
            get => _maintenanceMessageTextAccessor(ref _multiplayerModeSelectionView);
            set => _maintenanceMessageTextAccessor(ref _multiplayerModeSelectionView) = value;
        }

        private MasterServerQuickPlaySetupData _quickPlaySetupData
        {
            get => _quickPlaySetupDataAccessor(ref _multiplayerModeSelectionFlow);
            set => _quickPlaySetupDataAccessor(ref _multiplayerModeSelectionFlow) = value;
        }

        [Inject]
        internal void Inject(
            ServerDetailProvider serverDetails,
            IUnifiedNetworkPlayerModel networkPlayerModel,
            IMasterServerAvailabilityModel serverAvailability,
            IMasterServerQuickPlaySetupModel serverQuickPlaySetupModel,
            MultiplayerModeSelectionViewController multiplayerModeSelectionView,
            MultiplayerModeSelectionFlowCoordinator multiplayerModeSelectionFlow)
        {
            _serverDetails = serverDetails;
            _networkPlayerModel = networkPlayerModel;
            _serverAvailabilityModel = serverAvailability;
            _serverQuickPlaySetupModel = serverQuickPlaySetupModel;
            _multiplayerModeSelectionView = multiplayerModeSelectionView;
            _multiplayerModeSelectionFlow = multiplayerModeSelectionFlow;
        }

        public void Initialize()
        {
            _multiplayerModeSelectionView.didActivateEvent += _multiplayerModeSelectionView_didActivateEvent;
            gameObject.transform.SetParent(_multiplayerModeSelectionView.transform.parent, false);
        }

        public void Dispose()
        {
            _multiplayerModeSelectionView.didActivateEvent -= _multiplayerModeSelectionView_didActivateEvent;
        }

        private void _multiplayerModeSelectionView_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        }

        private async Task RefreshMasterServer()
        {
            _networkPlayerModel.ResetMasterServerReachability();
            MasterServerAvailabilityData availabilityData = await _serverAvailabilityModel.GetAvailabilityAsync(CancellationToken.None);
            _quickPlaySetupData = await _serverQuickPlaySetupModel.GetQuickPlaySetupAsync(CancellationToken.None);

            string statusText = availabilityData.GetLocalizedMessage(Localization.Instance.SelectedLanguage) ?? availabilityData.status switch
            {
                MasterServerAvailabilityData.AvailabilityStatus.Online
                    => "<color=\"green\">ONLINE",
                MasterServerAvailabilityData.AvailabilityStatus.MaintenanceUpcoming
                    => "<color=\"yellow\">MAINTENANCE UPCOMING",
                MasterServerAvailabilityData.AvailabilityStatus.Offline
                    => "<color=\"green\">ONLINE"
            };

            _maintenanceMessageText.richText = true;
            _maintenanceMessageText.SetText($"Status: {statusText}");

            // TODO: Loading thingy?
        }

        [UIValue("server-options")]
        private List<ServerDetails> servers => _serverDetails.Servers;

        [UIValue("server-selection")]
        private ServerDetails selectedServer
        {
            get => _serverDetails.SelectedServer;
            set
            {
                _serverDetails.SelectedServer = value;
                _ = RefreshMasterServer();
                NotifyPropertyChanged(nameof(selectedServer));
            }
        }
    }
}
