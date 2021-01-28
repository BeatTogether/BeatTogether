using System.Collections.Generic;
using System.Linq;
using BeatTogether.Models;

namespace BeatTogether.Providers
{
    internal class ServerDetailProvider
    {
        public static ServerDetailProvider Instance { get; }
            = new ServerDetailProvider();

        public List<ServerDetails> Servers { get; private set; }
        public ServerDetails Selection { get; set; }

        public ServerDetailProvider()
        {
            Servers = Plugin.Configuration.Servers
                .Append(ServerDetails.CreateOfficialInstance())
                .ToList();
            Selection = Servers.FirstOrDefault(server => server.Is(Plugin.Configuration.SelectedServer));
            if (Selection == null)
                Selection = Servers.First();
        }
    }
}
