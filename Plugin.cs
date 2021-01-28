using System.Reflection;
using BeatTogether.Configuration;
using BeatTogether.Providers;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace BeatTogether
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private const string _harmonyId = "com.Python.BeatTogether";
        internal static Harmony Harmony { get; private set; }
        internal static PluginConfiguration Configuration { get; private set; }
        internal static IPALogger Logger { get; private set; }
        internal static ServerDetailProvider ServerDetailProvider { get => ServerDetailProvider.Instance; }
        internal static ServerStatusProvider StatusProvider { get; private set; }

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
            StatusProvider = new ServerStatusProvider();
        }

        [OnEnable]
        public void OnEnable()
            => Harmony.PatchAll(Assembly.GetExecutingAssembly());

        [OnDisable]
        public void OnDisable()
            => Harmony.UnpatchAll(_harmonyId);
    }
}
