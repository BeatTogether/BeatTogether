using System.Collections.Generic;
using System.Linq;
using BeatTogether.Models;

namespace BeatTogether.Providers
{
    public class ServerDetailProvider
    {
        public List<ServerDetails> Servers { get; protected set; }
        public ServerDetails SelectedServer
        {
            get => GetServerDetails(_config.SelectedServer);
            set => _config.SelectedServer = value.ServerName;
        }

        private readonly Config _config;

        internal ServerDetailProvider(Config config)
        {
            _config = config;
            Servers = config.Servers;
            Servers.Add(new ServerDetails { ServerName = ServerDetails.OfficialServerName });
        }

        public ServerDetails GetServerDetails(string serverName)
            => Servers.FirstOrDefault(server => server.ServerName == serverName);
    }
}
