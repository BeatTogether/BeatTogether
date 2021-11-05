using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeatTogether.Models;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace BeatTogether
{
    public class Config
    {
        public virtual string SelectedServer { get; set; } = ServerDetails.BeatTogetherServerName;

        [NonNullable, UseConverter(typeof(CollectionConverter<ServerDetails, List<ServerDetails>>))]
        public virtual List<ServerDetails> Servers { get; set; } = new List<ServerDetails>();

        public virtual void OnReload()
        {
            if (!Servers.Any(server => 
                server.ServerName != ServerDetails.BeatTogetherServerName))
            {
                Servers.Add(new ServerDetails
                {
                    ServerName = ServerDetails.BeatTogetherServerName,
                    HostName = "master.beattogether.systems",
                    StatusUri = "http://master.beattogether.systems/status"
                });
            }
        }

        public virtual void Changed()
        {
        }

        public virtual void CopyFrom(Config other)
        {
            SelectedServer = other.SelectedServer;
            Servers = other.Servers;
        }
    }
}
