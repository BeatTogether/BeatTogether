using BeatSaberMarkupLanguage.Components.Settings;
using BeatTogether.UI;
using SiraUtil.Logging;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatTogether.Installers
{
    class BtMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ServerSelectionController>().FromNewComponentOnNewGameObject().AsSingle();
        }
    }
}
