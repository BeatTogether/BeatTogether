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

        public void SetServerStatus(string serverId, MasterServerAvailabilityData status)
        {
            _serverStatus.Remove(serverId);
            _serverStatus.Add(serverId, status);
        }

        public MasterServerAvailabilityData GetServerStatus(string serverId)
        {
            if (_serverStatus.TryGetValue(serverId, out var status))
                return status;
            return null;
        }
    }
}
