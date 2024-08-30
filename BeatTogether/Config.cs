using System.Collections.Generic;
using System.Linq;
using BeatTogether.Models;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace BeatTogether
{
    public class Config
    {
        // Official master server name that will be seen by players
        public const string OfficialServerName = "Official Servers";

        // BeatTogether master server config
        public const int DefaultApiPort = 8989;
        public const string BeatTogetherServerName = "BeatTogether";
        public const string BeatTogetherHostName = "master.beattogether.systems";
        public const string BeatTogetherApiUri = "http://master.beattogether.systems:8989";
        public const string BeatTogetherStatusUri = "http://master.beattogether.systems/status";
        public const int BeatTogetherMaxPartySize = 100;

        public virtual string SelectedServer { get; set; } = BeatTogetherServerName;

        [NonNullable, UseConverter(typeof(CollectionConverter<ServerDetails, List<ServerDetails?>>))]
        public virtual List<ServerDetails> Servers { get; set; } = new();

        public virtual void OnReload()
        {
            var haveBtServer = false;
            
            foreach (var server in Servers)
            {
                if (server.ServerName == BeatTogetherServerName)
                    haveBtServer = true;
                
                // Try to auto migrate API URL if missing from older configs
                if (string.IsNullOrEmpty(server.ApiUrl))
                    server.ApiUrl = $"http://{server.HostName}:{DefaultApiPort}";
            }

            if (!haveBtServer)
            {
                Servers.Insert(0, new ServerDetails
                {
                    ServerName = BeatTogetherServerName,
                    HostName = BeatTogetherHostName,
                    ApiUrl = BeatTogetherApiUri,
                    StatusUri = BeatTogetherStatusUri,
                    MaxPartySize = BeatTogetherMaxPartySize,
                    DisableSsl = true
                });
            }
        }

        public virtual void CopyFrom(Config other)
        {
            SelectedServer = other.SelectedServer;
            Servers = other.Servers;
        }
    }
}
