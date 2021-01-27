using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatTogether.Configuration;

namespace BeatTogether.Model
{
    internal class MasterServerProvider
    {
        public List<ServerDetails> Servers { get; private set; }
        public ServerDetails Selection { get; set; }

        public MasterServerProvider(List<ServerDetails> endpoints, string selected)
        {
            List<ServerDetails> servers = new List<ServerDetails>();

            Selection = ServerDetails.CreateOfficialInstance();
            servers.Add(Selection);

            foreach (var server in endpoints)
            {
                servers.Add(server);
                if (server.Is(selected))
                {
                    Selection = server;
                }
            }

            Servers = servers;
        }
    }
}
