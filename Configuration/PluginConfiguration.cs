using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatTogether.Configuration
{
    internal class PluginConfiguration
    {
        public static PluginConfiguration Instance { get; set; }

        public virtual bool Enabled { get; set; } = true;
        public virtual string HostName { get; set; } = "btogether.xn--9o8hpe.ws";
        public virtual int Port { get; set; } = 2328;

        public virtual void OnReload()
        {
        }

        public virtual void Changed()
        {
        }

        public virtual void CopyFrom(PluginConfiguration other)
        {
            Enabled = other.Enabled;
            HostName = other.HostName;
            Port = other.Port;
        }
    }
}
