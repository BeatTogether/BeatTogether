using System;

namespace BeatTogether.Models
{
    public class TemporaryServerDetails : ServerDetails
    {
        public TemporaryServerDetails(string apiUrl, int masterServerPort)
        {
            var urlParsed = new Uri(apiUrl);

            ServerName = urlParsed.Host;
            HostName = urlParsed.Host;
            Port = masterServerPort;
            StatusUri = string.Empty;
            MaxPartySize = IsOfficial ? 5 : 128;
            ApiPort = urlParsed.Port;
            ApiSecure = urlParsed.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}