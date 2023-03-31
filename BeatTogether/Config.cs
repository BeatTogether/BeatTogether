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
        public const string BeatTogetherServerName = "BeatTogether";
        public const string BeatTogetherHostName = "master.beattogether.systems";
        public const string BeatTogetherStatusUri = "http://master.beattogether.systems/status";
        public const int BeatTogetherMaxPartySize = 100;
        public const int BeatTogetherApiPort = 80;
        public const bool BeatTogetherApiSecure = false;

        public virtual string SelectedServer { get; set; } = BeatTogetherServerName;

        [NonNullable, UseConverter(typeof(CollectionConverter<ServerDetails, List<ServerDetails?>>))]
        public virtual List<ServerDetails> Servers { get; set; } = new();

        public virtual void OnReload()
        {
            var btServer = Servers.FirstOrDefault(server => server.ServerName == BeatTogetherServerName);

            if (btServer != null)
                return;
            
            Servers.Insert(0, new ServerDetails
            {
                ServerName = BeatTogetherServerName,
                HostName = BeatTogetherHostName,
                StatusUri = BeatTogetherStatusUri,
                MaxPartySize = BeatTogetherMaxPartySize,
                ApiPort = BeatTogetherApiPort,
                ApiSecure = BeatTogetherApiSecure
            });
        }

        public virtual void CopyFrom(Config other)
        {
            SelectedServer = other.SelectedServer;
            Servers = other.Servers;
        }
    }
}
