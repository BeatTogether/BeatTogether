using MasterServer;

namespace BeatTogether.Providers
{
    internal class GameClassInstanceProvider
    {
        public static GameClassInstanceProvider Instance { get; }
            = new GameClassInstanceProvider();

        private GameClassInstanceProvider()
        {
        }

        public UserMasterServerMessageHandler UserMessageHandler { get; set; }
        public MultiplayerModeSelectionViewController MultiplayerModeSelectionViewController { get; set; }
        public QuickPlaySetupModel MasterServerQuickPlaySetupModel { get; set; }
        public QuickPlaySongPacksDropdown QuickPlaySongPacksDropdown { get; set; }

        public MultiplayerModeSelectionFlowCoordinator MultiplayerModeSelectionFlowCoordinator { get; set; }
        public JoinQuickPlayViewController JoinQuickPlayViewController { get; set; }
        public JoiningLobbyViewController JoiningLobbyViewController { get; set; }

    }
}
