namespace BeatTogether.Models
{
    public class TemporaryServerDetails : ServerDetails
    {
        public TemporaryServerDetails(DnsEndPoint masterServerEndPoint)
        {
            ServerName = masterServerEndPoint.hostName;
            HostName = masterServerEndPoint.hostName;
            Port = masterServerEndPoint.port;
            StatusUri = string.Empty;
            MaxPartySize = IsOfficial ? 5 : 128;
        }
    }
}