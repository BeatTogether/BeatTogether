using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeatTogether.Models;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatTogether.Configuration
{
    internal class PluginConfiguration
    {
        public static PluginConfiguration Instance { get; set; }

        public virtual string SelectedServer { get; set; } = ServerDetails.BEAT_TOGETHER_SERVER_NAME;

        [NonNullable, UseConverter(typeof(CollectionConverter<ServerDetails, List<ServerDetails>>))]
        public virtual List<ServerDetails> Servers { get; set; } = new List<ServerDetails>();

        public virtual void OnReload()
        {
            Servers = Servers
                .Where(server => server.ServerName != ServerDetails.BEAT_TOGETHER_SERVER_NAME &&
                                 server.ServerName != ServerDetails.OFFICIAL_SERVER_NAME)
                .Prepend(new ServerDetails
                {
                    ServerName = ServerDetails.BEAT_TOGETHER_SERVER_NAME,
                    HostName = "master.beattogether.systems",
                    StatusUri = "http://master.beattogether.systems/status"
                })
                .ToList();
        }

        public virtual void Changed()
        {
        }

        public virtual void CopyFrom(PluginConfiguration other)
        {
            SelectedServer = other.SelectedServer;
            Servers = other.Servers;
        }
    }
}
