using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatTogether.Configuration;

namespace BeatTogether.UI
{
    internal class ServerSelectionController
    {
        private readonly MultiplayerModeSelectionViewController _multiplayerView;
        public ServerSelectionController(ListSetting listSetting, MultiplayerModeSelectionViewController multiplayerView)
        {
            _multiplayerView = multiplayerView;
            BSMLAction changed = new BSMLAction(this, typeof(ServerSelectionController).GetMethod("OnServerChanged"));
            listSetting.onChange = changed;
            UpdateUI(multiplayerView, Plugin.ServerProvider.Selection);
        }

        public void OnServerChanged(object selection)
        {
            ServerDetails details = selection as ServerDetails;
            Plugin.Logger.Info($"Server selection has changed to {details.ServerName} ({details.ServerId})");
            Plugin.Configuration.SelectedServer = details.ServerId;
            Plugin.ServerProvider.Selection = details;

            UpdateUI(_multiplayerView, details);
        }

        #region private
        private void UpdateUI(MultiplayerModeSelectionViewController multiplayerView, ServerDetails details)
        {
            var transform = _multiplayerView.transform;
            var quickPlayButton = transform.Find("Buttons/QuickPlayButton").gameObject;
            if (quickPlayButton == null)
            {
                Plugin.Logger.Error("QuickPlayButton not found");
                return;
            }
            quickPlayButton.SetActive(details.IsOfficial);
        }
        #endregion
    }
}
