using BeatTogether.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatTogether.Registries
{
    public class ServerDetailsRegistry
    {
        public ServerDetails SelectedServer 
            => Servers.FirstOrDefault(details => details.ServerName == _config.SelectedServer)
            ?? Servers.FirstOrDefault(details => details.ServerName == Config.BeatTogetherServerName);

        public IReadOnlyList<ServerDetails> Servers
            => _config.Servers.Concat(_servers).ToList();

        private readonly Config _config;
        private readonly List<ServerDetails> _servers = new();

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
            => _config.SelectedServer = server.ServerName;
    }
}
