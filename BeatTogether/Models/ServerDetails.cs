namespace BeatTogether.Models
{
    public class ServerDetails
    {
        public string ServerName { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; } = 2328;
        public string StatusUri { get; set; } = string.Empty;
        public int MaxPartySize { get; set; } = 5;

        public MasterServerEndPoint? EndPoint => string.IsNullOrEmpty(ServerName) ? null : new(HostName, Port);
        public bool IsOfficial => ServerName == Config.OfficialServerName;

        public override string ToString() => ServerName;
    }
}
