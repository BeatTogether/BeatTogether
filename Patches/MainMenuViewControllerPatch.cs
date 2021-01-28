using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MainMenuViewController), "DidActivate")]
    internal class DidActivatePatch
    {
        internal static Vector3 DefaultTextPosition { get; set; }
        internal static string DefaultText { get; set; }

        internal static void Postfix(MainMenuViewController __instance, bool firstActivation)
        {
            if (!firstActivation)
                return;

            var transform = __instance.gameObject.transform;
            var onlineButton = transform.Find("MainButtons/OnlineButton").gameObject;
            var onlineButtonTextObj = onlineButton.transform.Find("Text").gameObject;
            var onlineButtonText = onlineButtonTextObj.GetComponent<TextMeshProUGUI>();
            DefaultTextPosition = onlineButtonTextObj.transform.position;

            onlineButtonTextObj.transform.position = new Vector3(
                DefaultTextPosition.x + 0.025f,
                DefaultTextPosition.y,
                DefaultTextPosition.z
            );
            onlineButtonText.SetText("Modded Online");
        }
    }
}
