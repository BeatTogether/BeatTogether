using BeatTogether.UI;
using Zenject;

namespace BeatTogether.Installers
{
    class BtMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ServerSelectionController>().AsSingle();
        }
    }
}
