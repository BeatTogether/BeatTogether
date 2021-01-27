using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace BeatTogether.Model
{
    internal class ServerDetails
    {
        private readonly static int DEFAULT_PORT = 2328;
        private static readonly string OFFICIAL_SERVER_ID = null;
        private MasterServerEndPoint _endPoint;

        public string ServerName { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; } = DEFAULT_PORT;
        public string StatusUri { get; set; }
        public string ServerId { get; set; }

        public bool IsOfficial { get => ServerId == OFFICIAL_SERVER_ID; }

        public override string ToString()
        {
            return ServerName;
        }

        public MasterServerEndPoint GetEndPoint()
        {
            if (ServerId == OFFICIAL_SERVER_ID)
            {
                return null;
            }

            if (_endPoint == null)
            {
                _endPoint = new MasterServerEndPoint(HostName, Port);
            }

            return _endPoint;
        }

        public bool Is(string serverId)
        {
            return serverId == ServerId;
        }
    }
    internal class MasterServerProvider
    {
        private readonly static string OFFICIAL_SERVERS = "Official Servers";
        public List<ServerDetails> Servers { get; private set; }
        public ServerDetails Selection { get; set; }

        public MasterServerProvider(List<ServerDetails> endpoints, string selected)
        {
            List<ServerDetails> servers = new List<ServerDetails>();

            Selection = new ServerDetails()
            {
                ServerName = OFFICIAL_SERVERS
            };
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
