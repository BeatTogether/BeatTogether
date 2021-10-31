using System.Linq;
using BeatSaberMarkupLanguage.Components.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatTogether.UI
{
    public class UIFactory
    {
        internal static ListSetting CreateServerSelectionView(MultiplayerModeSelectionViewController multiplayerView)
        {
            Plugin.Logger.Info("Applying interface for server selection.");
            var parent = multiplayerView.gameObject.transform;
            FormattedFloatListSettingsValueController baseSetting = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<FormattedFloatListSettingsValueController>().First(x => (x.name == "VRRenderingScale")), parent, false);
            baseSetting.name = "BSMLIncDecSetting";

            GameObject gameObject = baseSetting.gameObject;
            MonoBehaviour.Destroy(baseSetting);
            gameObject.SetActive(false);

            ListSetting serverSelection = gameObject.AddComponent<ListSetting>();
            gameObject.transform.position += new Vector3(0.0f, 0.15f, 0.0f);
            gameObject.transform.GetChild(1).position += new Vector3(-1.0f, 0.0f, 0.0f);

            serverSelection.text = gameObject.transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>().First();
            serverSelection.text.richText = true;
            serverSelection.decButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().First();
            serverSelection.incButton = gameObject.transform.GetChild(1).GetComponentsInChildren<Button>().Last();
            (gameObject.transform.GetChild(1) as RectTransform).sizeDelta = new Vector2(60, 0);
            serverSelection.text.overflowMode = TextOverflowModes.Ellipsis;

            var controller = multiplayerView.gameObject.AddComponent<ServerSelectionController>();
            controller.Init(serverSelection, multiplayerView);

            TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.transform.position += new Vector3(1.2f, 0.0f, 0.0f);
            text.SetText("Playing on");
            text.richText = true;

            gameObject.GetComponent<LayoutElement>().preferredWidth = 90;
            gameObject.SetActive(true);

            // Initial Update for the SongPacks
            Patches.QuickPlaySongPacksDropdownPatch.UpdateSongPacks();
            return serverSelection;
        }
    }
}
