using BeatSaberMarkupLanguage.Components.Settings;
using BeatTogether.UI;
using SiraUtil.Logging;
using Zenject;

namespace BeatTogether.Installers
{
    class BtMenuInstaller : Installer
    {
        private readonly Config _config;

        public BtMenuInstaller(
            Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();
            Container.BindInterfacesAndSelfTo<ServerSelectionController>().AsSingle();
        }
    }
}
