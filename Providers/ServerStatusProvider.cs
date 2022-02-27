using System.Collections.Generic;

namespace BeatTogether.Providers
{
    internal class ServerStatusProvider
    {
        private readonly Dictionary<string, MultiplayerStatusData> _serverStatus;

        public ServerStatusProvider()
        {
            _serverStatus = new Dictionary<string, MultiplayerStatusData>();
        }

        public void SetServerStatus(string serverName, MultiplayerStatusData status) =>
            _serverStatus[serverName] = status;

        public MultiplayerStatusData GetServerStatus(string serverName)
        {
            if (_serverStatus.TryGetValue(serverName, out var status))
                return status;
            return null;
        }
    }
}
