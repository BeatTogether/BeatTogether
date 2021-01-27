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
        public Dictionary<string, MasterServerAvailabilityData> ServerStatus { get; } = new Dictionary<string, MasterServerAvailabilityData>();

        public ServerStatusProvider()
        {
        }

        internal void SetServerStatus(string key, MasterServerAvailabilityData value)
        {
            ServerStatus.Remove(key);
            ServerStatus.Add(key, value);
        }
    }
}
