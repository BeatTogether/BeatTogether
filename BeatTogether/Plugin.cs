using BeatTogether.Installers;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace BeatTogether
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    class Plugin
    {
        private readonly Harmony _harmony;
        private readonly PluginMetadata _metadata;
        public const string ID = "com.Python.BeatTogether";

        [Init]
        public Plugin(IPALogger logger, Conf conf, PluginMetadata metadata, Zenjector zenjector)
        {
            Config config = conf.Generated<Config>();

            _harmony = new Harmony(ID);
            _metadata = metadata;

            zenjector.UseLogger(logger);
            zenjector.Install<BtMenuInstaller>(Location.Menu, config);
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony.PatchAll(_metadata.Assembly);
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchAll(ID);
        }
    }
}
