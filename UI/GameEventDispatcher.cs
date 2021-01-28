using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatTogether.UI
{
    internal class GameEventDispatcher
    {
        public static GameEventDispatcher Instance { get; private set; } = new GameEventDispatcher();

        public EventHandler<MultiplayerModeSelectionViewController> MultiplayerViewEntered;

        private GameEventDispatcher()
        {
        }

        internal void OnMultiplayerViewEntered(MultiplayerModeSelectionViewController instance)
        {
            MultiplayerViewEntered?.Invoke(this, instance);
        }
    }
}
