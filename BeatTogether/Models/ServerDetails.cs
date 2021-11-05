namespace BeatTogether.Models
{
    public class ServerDetails
    {
        public const string OfficialServerName = "Official Servers";
        public const string BeatTogetherServerName = "BeatTogether";

        public const int DefaultPort = 2328;

        public string ServerName { get; set; } = null;
        public string? HostName { get; set; } = null;
        public int Port { get; set; } = DefaultPort;
        public string? StatusUri { get; set; } = null;
        public bool IsOfficial => ServerName == OfficialServerName;
        public MasterServerEndPoint? EndPoint => IsOfficial ? null : new(HostName, Port);

        public override string ToString() => ServerName;

        public bool Is(string serverName) => ServerName == serverName;
    }
}
