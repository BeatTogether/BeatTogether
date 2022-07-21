using BeatTogether.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatTogether.Registries
{
    public class ServerDetailsRegistry
    {
        public ServerDetails SelectedServer
            => TemporarySelectedServer
            ?? Servers.FirstOrDefault(details => details.ServerName == _config.SelectedServer)
            ?? Servers.FirstOrDefault(details => details.ServerName == Config.BeatTogetherServerName);

        public IReadOnlyList<ServerDetails> Servers
            => _config.Servers.Concat(_servers).Append(OfficialServer).ToList();

        private readonly Config _config;
        private readonly List<ServerDetails> _servers = new();
        
        public readonly ServerDetails OfficialServer = new()
        {
            ServerName = Config.OfficialServerName
        };

        public TemporaryServerDetails? TemporarySelectedServer { get; private set; }

        internal ServerDetailsRegistry(
            Config config)
        {
            _config = config;
        }

        public void AddServer(ServerDetails server)
        {
            if (Servers.Any(details => details.ServerName == server.ServerName))
                throw new ArgumentException($"A server already exists with the name {server.ServerName}.");
            _servers.Add(server);
        }

        public void SetSelectedServer(ServerDetails server)
        {
            if (server is TemporaryServerDetails tmpServer)
            {
                TemporarySelectedServer = tmpServer;
            }
            else
            {
                _config.SelectedServer = server.ServerName;
                TemporarySelectedServer = null;
            }
        }
    }
}
