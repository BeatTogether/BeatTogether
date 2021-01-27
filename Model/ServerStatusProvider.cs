using BeatTogether.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatTogether.Model
{
    internal class ServerStatusProvider
    {
        private List<ServerDetails> servers;
        public Dictionary<string, MasterServerAvailabilityData> ServerStatus { get; } = new Dictionary<string, MasterServerAvailabilityData>();

        public ServerStatusProvider(List<ServerDetails> servers)
        {
        }
    }
}
