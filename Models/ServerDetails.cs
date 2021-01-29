namespace BeatTogether.Models
{
    internal class ServerDetails
    {
        public readonly static int DEFAULT_PORT = 2328;
        public readonly static string OFFICIAL_SERVER_NAME = "Official Servers";

        public static string OfficialStatusUri { set; get; }

        private MasterServerEndPoint _endPoint;

        public string ServerName { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; } = DEFAULT_PORT;
        public string StatusUri { get; set; }
        public bool IsOfficial { get => ServerName == OFFICIAL_SERVER_NAME; }

        public override string ToString() => ServerName;

        public MasterServerEndPoint GetEndPoint()
        {
            if (ServerName == OFFICIAL_SERVER_NAME)
                return null;
            if (_endPoint == null)
                _endPoint = new MasterServerEndPoint(HostName, Port);
            return _endPoint;
        }

        public bool Is(string serverName) => ServerName == serverName;

        internal static ServerDetails CreateOfficialInstance() =>
            new ServerDetails
            {
                ServerName = OFFICIAL_SERVER_NAME,
                StatusUri = OfficialStatusUri
            };
    }
}
