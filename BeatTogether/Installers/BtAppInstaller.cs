using BeatTogether.Registries;
using Zenject;

namespace BeatTogether.Installers
{
    class BtAppInstaller : Installer
    {
        private readonly Config _config;

        public BtAppInstaller(
            Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();
            Container.BindInterfacesAndSelfTo<ServerDetailsRegistry>().AsSingle();
        }
    }
}
