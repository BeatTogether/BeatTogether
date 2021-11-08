using BeatSaberMarkupLanguage.Components.Settings;
using BeatTogether.UI;
using SiraUtil.Logging;
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
