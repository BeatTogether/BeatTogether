using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MainMenuViewController), "DidActivate")]
    internal class DidActivatePatch
    {
        internal static bool Prefix(MainMenuViewController __instance)
        {
            var transform = __instance.gameObject.transform;
            var onlineButton = transform.Find("MainButtons/OnlineButton").gameObject;
            var onlineButtonTextObj = onlineButton.transform.Find("Text").gameObject;
            onlineButtonTextObj.transform.position = new Vector3(
                0.43744f,
                onlineButtonTextObj.transform.position.y,
                onlineButtonTextObj.transform.position.z
            );
            var onlineButtonText = onlineButtonTextObj.GetComponent<TextMeshProUGUI>();
            onlineButtonText.SetText("Modded Online");
            return true;
        }
    }
}
