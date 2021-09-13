using System.Reflection;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatTogether.Models;
using BeatTogether.Providers;
using IPA.Utilities;
using MasterServer;
using TMPro;
using UnityEngine;

namespace BeatTogether.UI
{
    internal class ServerSelectionController : MonoBehaviour
    {
        private static MethodInfo _unauthenticateWithMasterServerMethodInfo = typeof(UserMessageHandler)
            .GetMethod("UnauthenticateWithMasterServer", BindingFlags.Instance | BindingFlags.NonPublic);

        private MultiplayerModeSelectionViewController _multiplayerView;

        public void Init(ListSetting listSetting, MultiplayerModeSelectionViewController multiplayerView)
        {
            _multiplayerView = multiplayerView;
            var changed = new BSMLAction(this, typeof(ServerSelectionController).GetMethod("OnServerChanged"));
            listSetting.onChange = changed;
            UpdateUI(multiplayerView, Plugin.ServerDetailProvider.Selection);
            GameEventDispatcher.Instance.MultiplayerViewEntered += OnMultiplayerViewEntered;
        }

        public void OnDestroy()
            => GameEventDispatcher.Instance.MultiplayerViewEntered -= OnMultiplayerViewEntered;

        public void OnServerChanged(object selection)
        {
            ServerDetails details = selection as ServerDetails;
            Plugin.Configuration.SelectedServer = details.ServerName;
            Plugin.ServerDetailProvider.Selection = details;

            // Keep this code, as it informs MPEX of the change
            // (by invoking the getters):
            var networkConfig = GetNetworkConfig();
            var endPoint = networkConfig.masterServerEndPoint;
            var statusUrl = networkConfig.masterServerStatusUrl;
            Plugin.Logger.Debug(
                "Master server selection changed " +
                $"(EndPoint={endPoint}, StatusUrl={statusUrl})"
            );

            DisconnectServer();
            UpdateUI(_multiplayerView, details);
        }

        #region Private Methods

        private void DisconnectServer()
        {
            var handler = GameClassInstanceProvider.Instance.UserMessageHandler;
            if (handler == null)
                return;
            _unauthenticateWithMasterServerMethodInfo.Invoke(handler, new object[] { });
        }

        private void UpdateUI(MultiplayerModeSelectionViewController multiplayerView, ServerDetails details)
        {
            var status = Plugin.StatusProvider.GetServerStatus(details.ServerName);
            multiplayerView.SetData(status);

            var textMesh = GetMaintenanceMessageText();
            if (textMesh.gameObject.activeSelf)
            {
                textMesh.richText = false;
                return;
            }

            textMesh.richText = true;
            if (status == null)
            {
                textMesh.SetText("Status: <color=\"yellow\">UNKNOWN");
                textMesh.gameObject.SetActive(true);
                return;
            }

            switch (status.status)
            {
                case MasterServerAvailabilityData.AvailabilityStatus.Offline:
                    textMesh.SetText("Status: <color=\"red\">OFFLINE");
                    break;
                case MasterServerAvailabilityData.AvailabilityStatus.MaintenanceUpcoming:
                    textMesh.SetText("Status: <color=\"yellow\">MAINTENANCE UPCOMING");
                    break;
                case MasterServerAvailabilityData.AvailabilityStatus.Online:
                    textMesh.SetText("Status: <color=\"green\">ONLINE");
                    break;
            }
            textMesh.gameObject.SetActive(true);
        }

        private INetworkConfig GetNetworkConfig() =>
            ReflectionUtil.GetField<INetworkConfig, MultiplayerModeSelectionViewController>(_multiplayerView, "_networkConfig");

        private TextMeshProUGUI GetMaintenanceMessageText() =>
            ReflectionUtil.GetField<TextMeshProUGUI, MultiplayerModeSelectionViewController>(_multiplayerView, "_maintenanceMessageText");

        private void OnMultiplayerViewEntered(object sender, MultiplayerModeSelectionViewController multiplayerView)
        {
            var selection = Plugin.ServerDetailProvider.Selection;
            UpdateUI(multiplayerView, selection);
        }

        #endregion
    }
}
