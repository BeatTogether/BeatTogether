using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterServer;

namespace BeatTogether.Model
{
    internal class GameClassInstanceProvider
    {
        public static GameClassInstanceProvider Instance { get; private set; } = new GameClassInstanceProvider();

        protected GameClassInstanceProvider()
        {
        }

        public UserMessageHandler UserMessageHandler { get; set; }
    }
}
