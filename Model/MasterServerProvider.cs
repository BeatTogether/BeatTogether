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
        private static MasterServerProvider _instance;
        public List<ServerDetails> Servers { get; private set; }
        public ServerDetails Selection { get; set; }

        public static MasterServerProvider Instance
        { 
            get
            {
                if (_instance == null)
                {
                    _instance = new MasterServerProvider();
                }
                return _instance;
            }
        }

        private MasterServerProvider()
        {
            var endpoints = Plugin.Configuration.Servers;
            var selected = Plugin.Configuration.SelectedServer;
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
