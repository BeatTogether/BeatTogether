using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MainMenuViewController), "DidActivate")]
    internal class DidActivatePatch
    {
        internal static bool ConfigChanged { get; set; } = false;
        internal static Vector3 DefaultTextPosition { get; set; }
        internal static string DefaultText { get; set; }
        static DidActivatePatch()
        {
            Plugin.Configuration.ConfigChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e)
        {
            ConfigChanged = true;
        }
        internal static void Prefix(MainMenuViewController __instance, bool firstActivation)
        {
            if (!firstActivation && !ConfigChanged)
            {
                return;
            }
            ConfigChanged = false;

            var transform = __instance.gameObject.transform;
            var onlineButton = transform.Find("MainButtons/OnlineButton").gameObject;
            var onlineButtonTextObj = onlineButton.transform.Find("Text").gameObject;
            var onlineButtonText = onlineButtonTextObj.GetComponent<TextMeshProUGUI>();

            if (firstActivation)
            {
                DefaultTextPosition = onlineButtonTextObj.transform.position;
            }

            if (Plugin.Configuration.Enabled)
            {
                onlineButtonTextObj.transform.position = new Vector3(
                    DefaultTextPosition.x + 0.025f,
                    DefaultTextPosition.y,
                    DefaultTextPosition.z
                );
                onlineButtonText.SetText("Modded Online");
            }
            else
            {
                onlineButtonTextObj.transform.position = DefaultTextPosition;
                onlineButtonText.SetText("Online");
            }
        }
    }
}
