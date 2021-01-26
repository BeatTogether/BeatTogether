using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Settings;
using BeatTogether.Configuration;
using BeatTogether.UI;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using BeatTogether.Model;

namespace BeatTogether
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private const string _harmonyId = "com.Python.BeatTogether";
        internal static Harmony Harmony { get; private set; }
        internal static PluginConfiguration Configuration { get; private set; }
        internal static IPALogger Logger { get; private set; }
        internal static MasterServerProvider ServerProvider { get; set; }
        [OnStart]
        public void OnApplicationStart()
        {
        }

        [Init]
        public void Init(
            IPALogger logger,
            Config config)
        {
            Harmony = new Harmony(_harmonyId);
            Configuration = config.Generated<PluginConfiguration>();
            Logger = logger;
            ServerProvider = new MasterServerProvider(Configuration.Servers, Configuration.SelectedSever);
        }

        [OnEnable]
        public void OnEnable()
            => Harmony.PatchAll(Assembly.GetExecutingAssembly());

        [OnDisable]
        public void OnDisable()
            => Harmony.UnpatchAll(_harmonyId);
    }
}
