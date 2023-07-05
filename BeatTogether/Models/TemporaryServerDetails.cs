using System;

namespace BeatTogether.Models
{
    public class TemporaryServerDetails : ServerDetails
    {
        public TemporaryServerDetails(string graphApiUrl, string? statusUrl)
        {
            try
            {
                var urlParsed = new Uri(graphApiUrl);

                ServerName = urlParsed.Host;
                HostName = urlParsed.Host;
            }
            catch (UriFormatException)
            {
                ServerName = graphApiUrl;
                HostName = graphApiUrl;
            }
            
            ApiUrl = graphApiUrl;
            StatusUri = statusUrl ?? graphApiUrl;
            MaxPartySize = IsOfficial ? 5 : 128;
            DisableSsl = true;
        }
    }
}