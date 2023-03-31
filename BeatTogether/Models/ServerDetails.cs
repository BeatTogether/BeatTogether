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
        /// Hostname for master server and API url
        /// </summary>
        public string HostName { get; set; } = string.Empty;
        /// <summary>
        /// Port number for master/auth server
        /// </summary>
        public int Port { get; set; } = 2328;
        /// <summary>
        /// Optional status check URL for the server
        /// </summary>
        public string StatusUri { get; set; } = string.Empty;
        /// <summary>
        /// Max amount of players per instance 
        /// </summary>
        public int MaxPartySize { get; set; } = 5;
        /// <summary>
        /// HTTP port number for Graph API server
        /// </summary>
        public int ApiPort { get; set; } = 80;
        /// <summary>
        /// Whether Graph API requests should use HTTPS protocol
        /// </summary>
        public bool ApiSecure { get; set; } = false;

        public DnsEndPoint? EndPoint => string.IsNullOrEmpty(ServerName) ? null : new(HostName, Port);

        public string ApiUrl
        {
            get
            {
                var protocol = ApiSecure ? "https" : "http";
                return $"{protocol}://{HostName}:{ApiPort}";
            }
        }
        
        public bool IsOfficial => ServerName == Config.OfficialServerName;

        /// <summary>
        /// Gets whether this server matches against a given override URL.
        /// Only checks whether the hostname and port match.
        /// </summary>
        public bool MatchesApiUrl(string? apiUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
                return false;
            
            Console.WriteLine(apiUrl);

            try
            {
                var urlParsed = new Uri(apiUrl);
                
                Console.WriteLine($"parsed.Host = {urlParsed.Host}");
                Console.WriteLine($"parsed.Port = {urlParsed.Port}");
                
                return urlParsed.Host == HostName && urlParsed.Port == ApiPort;
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public override string ToString() => ServerName;
    }
}
