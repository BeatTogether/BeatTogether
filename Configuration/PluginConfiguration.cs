using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using BeatTogether.Model;
using System.Collections.Generic;
using System.Collections;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatTogether.Configuration
{
    internal class PluginConfiguration
    {
        public static PluginConfiguration Instance { get; set; }

        public virtual string SelectedServer { get; set; } = "btogether.xn--9o8hpe.ws:2328";

        [NonNullable, UseConverter(typeof(CollectionConverter<ServerDetails, List<ServerDetails>>))]
        public virtual List<ServerDetails> Servers { get; set; } = new List<ServerDetails>()
        {
            new ServerDetails()
            {
                ServerId = "btogether.xn--9o8hpe.ws:2328",
                ServerName = "BeatTogether",
                HostName = "btogether.xn--9o8hpe.ws",
                StatusUri = "http://btogether.xn--9o8hpe.ws/status"
            }
        };

        public virtual void OnReload()
        {
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
