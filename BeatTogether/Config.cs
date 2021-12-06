using System.Collections.Generic;
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
        public const int BeatTogetherMaxPartySize = 10;

        public virtual string SelectedServer { get; set; } = BeatTogetherServerName;

        [NonNullable, UseConverter(typeof(CollectionConverter<ServerDetail, List<ServerDetail>>))]
        public virtual List<ServerDetail> Servers { get; set; } = new List<ServerDetail>();

        public virtual void OnReload()
        {
            Servers.RemoveAll(server =>
                server.ServerName == BeatTogetherServerName);
            Servers.Add(new ServerDetail
            {
                ServerName = BeatTogetherServerName,
                HostName = BeatTogetherHostName,
                StatusUri = BeatTogetherStatusUri,
                MaxPartySize = BeatTogetherMaxPartySize
            });
        }

        public virtual void CopyFrom(Config other)
        {
            SelectedServer = other.SelectedServer;
            Servers = other.Servers;
        }
    }
}
