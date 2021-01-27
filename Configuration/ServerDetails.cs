using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatTogether.Configuration
{
    internal class ServerDetails
    {
        public readonly static int DEFAULT_PORT = 2328;
        public readonly static string OFFICIAL_SERVER_NAME = "Official Servers";
        public readonly static string OFFICIAL_SERVER_ID = "steam.production.mp.beatsaber.com:2328";

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

        internal static ServerDetails CreateOfficialInstance()
        {
            return new ServerDetails()
            {
                ServerName = OFFICIAL_SERVER_NAME,
                ServerId = OFFICIAL_SERVER_ID
            };
        }
    }
}
