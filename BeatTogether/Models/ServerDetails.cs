using System;
using System.Security.Policy;

namespace BeatTogether.Models
{
    public class ServerDetails
    {
        /// <summary>
        /// Display name for UI
        /// </summary>
        public string ServerName { get; set; } = string.Empty;
        /// <summary>
        /// Legacy hostname for master server (no longer used, except for automatic config migrations)
        /// </summary>
        public string HostName { get; set; } = string.Empty;
        /// <summary>
        /// The multiplayer API url / graph url 
        /// </summary>
        public string ApiUrl { get; set; } = string.Empty;
        /// <summary>
        /// Optional status check URL for the server
        /// </summary>
        public string StatusUri { get; set; } = string.Empty;
        /// <summary>
        /// Max amount of players per instance 
        /// </summary>
        public int MaxPartySize { get; set; } = 5;
        /// <summary>
        /// If set: disable SSL and certificate validation for all Ignorance/ENet client connections.
        /// </summary>
        public bool DisableSsl { get; set; } = true;

        public bool IsOfficial => ServerName == Config.OfficialServerName;

        /// <summary>
        /// Gets whether this server matches against a given override URL.
        /// Only checks whether the hostname and port match.
        /// </summary>
        public bool MatchesApiUrl(string? apiUrl)
        {
            if (apiUrl == ApiUrl)
                // Exact match
                return true;
            
            if (string.IsNullOrEmpty(apiUrl))
                return false;
            
            // Loose match
            try
            {
                var urlOurs = new Uri(ApiUrl);
                var urlTheirs = new Uri(apiUrl);
                
                return urlOurs.Host == urlTheirs.Host &&
                       urlOurs.Port == urlTheirs.Port;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        public override string ToString() => ServerName;
    }
}
