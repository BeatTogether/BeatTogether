using System;

namespace BeatTogether.UI
{
    internal class GameEventDispatcher
    {
        public static GameEventDispatcher Instance { get; }
            = new GameEventDispatcher();

        public EventHandler<MultiplayerModeSelectionViewController> MultiplayerViewEntered;

        private GameEventDispatcher()
        {
        }

        internal void OnMultiplayerViewEntered(MultiplayerModeSelectionViewController instance) =>
            MultiplayerViewEntered?.Invoke(this, instance);
    }
}
