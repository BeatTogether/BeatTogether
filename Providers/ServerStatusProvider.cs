using System.Collections.Generic;

namespace BeatTogether.Providers
{
    internal class ServerStatusProvider
    {
        private readonly Dictionary<string, MasterServerAvailabilityData> _serverStatus;

        public ServerStatusProvider()
        {
            _serverStatus = new Dictionary<string, MasterServerAvailabilityData>();
        }

        public void SetServerStatus(string serverName, MasterServerAvailabilityData status) =>
            _serverStatus[serverName] = status;

        public MasterServerAvailabilityData GetServerStatus(string serverName)
        {
            if (_serverStatus.TryGetValue(serverName, out var status))
                return status;
            return null;
        }
    }
}
