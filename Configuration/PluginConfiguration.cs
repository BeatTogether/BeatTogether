using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatTogether.Configuration
{
    internal class PluginConfiguration
    {
        public static PluginConfiguration Instance { get; set; }

        public virtual string StatusUrl { get; set; } = "http://btogether.xn--9o8hpe.ws/status";
        public virtual string SelectedSever { get; set; } = "btogether.xn--9o8hpe.ws:2328";
        public virtual string Servers { get; set; } = "btogether.xn--9o8hpe.ws:2328";

        public virtual void OnReload()
        {
        }

        public virtual void Changed()
        {
        }

        public virtual void CopyFrom(PluginConfiguration other)
        {
            StatusUrl = other.StatusUrl;
            SelectedSever = other.SelectedSever;
            Servers = other.Servers;
        }
    }
}
