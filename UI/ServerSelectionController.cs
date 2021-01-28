using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using IPA.Utilities;
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
            Plugin.Configuration.SelectedServer = details.ServerId;
            Plugin.ServerProvider.Selection = details;

            // keep this code, as it informs MPEX over the change:
            var networkConfig = GetNetworkConfig();
            var address = networkConfig.masterServerEndPoint;
            var status = networkConfig.masterServerStatusUrl;
            Plugin.Logger.Info($"Server selection has changed to {details.ServerName} (endpoint={address}, status={status})");

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

            var status = Plugin.StatusProvider.GetStatus(details);
            multiplayerView.SetData(status);

            do
            {
                var textMesh = GetMaintenanceMessageText();
                if (textMesh.gameObject.activeSelf)
                {
                    textMesh.richText = false;
                    continue;
                }

                textMesh.richText = true;
                if (status == null)
                {
                    textMesh.SetText("Status: <color=\"yellow\">UNKNOWN");
                    textMesh.gameObject.SetActive(true);
                    continue;
                }

                switch (status.status)
                {
                    case MasterServerAvailabilityData.AvailabilityStatus.Offline:
                    case MasterServerAvailabilityData.AvailabilityStatus.MaintenanceUpcoming:
                        textMesh.SetText("Status: <color=\"red\">OFFLINE");
                        break;
                    case MasterServerAvailabilityData.AvailabilityStatus.Online:
                        textMesh.SetText("Status: <color=\"green\">ONLINE");
                        break;
                }
                textMesh.gameObject.SetActive(true);
            }
            while (false);
        }

        private INetworkConfig GetNetworkConfig()
        {
            return ReflectionUtil.GetField<INetworkConfig, MultiplayerModeSelectionViewController>(_multiplayerView, "_networkConfig");
        }

        private TextMeshProUGUI GetMaintenanceMessageText()
        {
            return ReflectionUtil.GetField<TextMeshProUGUI, MultiplayerModeSelectionViewController>(_multiplayerView, "_maintenanceMessageText");
        }
        #endregion
    }
}
