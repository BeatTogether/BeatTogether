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
        public static readonly string OFFICIAL_SERVER_ID = null;
        public MasterServerEndPoint EndPoint { get; set; }
        public string ServerName { get; set; }
        public string ServerId {
            get
            {
                if (EndPoint == null)
                {
                    return null;
                }
                return $"{EndPoint.hostName}:{EndPoint.port}";
            }
        }
        public bool IsOfficial { get => ServerId == OFFICIAL_SERVER_ID; }
        public override string ToString()
        {
            return ServerName;
        }
    }
    internal class MasterServerProvider
    {
        private readonly static int DEFAULT_PORT = 2328;
        private readonly static string OFFICIAL_SERVERS = "Official Servers";
        public List<ServerDetails> Servers { get; private set; }
        public ServerDetails Selection { get; set; }

        public MasterServerProvider(string endpoints, string selected)
        {
            List<ServerDetails> servers = new List<ServerDetails>();

            Selection = new ServerDetails() {
                EndPoint = null,
                ServerName = OFFICIAL_SERVERS
            };
            servers.Add(Selection);

            foreach (var entry in endpoints.Split(','))
            {
                Plugin.Logger.Error($"Adding Server {entry}");
                var server = ParseServer(entry);
                if (server != null)
                {
                    servers.Add(server);
                    if (selected == server.ServerId)
                    {
                        Selection = server;
                    }
                }
            }
            Servers = servers;
        }

        #region private
        private ServerDetails ParseServer(string value)
        {
            var parts = value.Split(':');
            var hostname = parts[0];
            if (hostname.Equals(""))
            {
                return null;
            }

            int port;
            if (parts.Length < 2 || !int.TryParse(parts[1], out port))
            {
                port = DEFAULT_PORT;
            }

            string serverName = (parts.Length > 2) ? parts[2] : hostname;

            return new ServerDetails() {
                EndPoint = new MasterServerEndPoint(hostname, port),
                ServerName = serverName
            };
        }
        #endregion
    }
}
