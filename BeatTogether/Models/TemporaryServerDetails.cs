using System;

namespace BeatTogether.Models
{
    public class TemporaryServerDetails : ServerDetails
    {
        public TemporaryServerDetails(string apiUrl, int masterServerPort)
        {
            try
            {
                var urlParsed = new Uri(apiUrl);

                ServerName = urlParsed.Host;
                HostName = urlParsed.Host;
            }
            catch (UriFormatException)
            {
                ServerName = apiUrl;
                HostName = apiUrl;
            }
            
            ApiUrl = apiUrl;
            StatusUri = string.Empty;
            MaxPartySize = IsOfficial ? 5 : 128;
        }
    }
}