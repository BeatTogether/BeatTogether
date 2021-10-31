using IPA.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatTogether.Providers
{
    internal class ModStatusProvider
    {
        public static bool ShouldBlockSongPackOverrides 
        { 
            get {
                var pluginMetadata = PluginManager.GetPluginFromId("MultiplayerExtensions");
                bool valid = ((pluginMetadata == null || !PluginManager.IsEnabled(pluginMetadata)) && !Plugin.ServerDetailProvider.Selection.IsOfficial);
                if (valid) Plugin.Logger.Info("On Official Servers or not modded with MpEx!");
                return valid;
                }
        }
    }
}
