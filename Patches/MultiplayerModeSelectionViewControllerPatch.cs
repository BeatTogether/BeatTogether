using System;
using System.Linq;
using HarmonyLib;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Tags.Settings;
using BeatTogether.UI;
using BeatTogether.Model;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MultiplayerModeSelectionViewController), "DidActivate")]
    internal class DidActivatePatch2
    {
        internal static void Postfix(MultiplayerModeSelectionViewController __instance, bool firstActivation)
        {
            GameEventDispatcher.Instance.OnMultiplayerViewEntered(__instance);

            if (firstActivation)
            {
                GameClassInstanceProvider.Instance.MultiplayerModeSelectionViewController = __instance;
                AddServerSelection(__instance);
            }
        }

        private static void AddServerSelection(MultiplayerModeSelectionViewController __instance)
        {
            var servers = Plugin.ServerProvider.Servers;
            var serverSelection = UiFactory.CreateServerSelectionView(__instance);

            serverSelection.values = servers.ToList<object>();
            serverSelection.Value = Plugin.ServerProvider.Selection;
        }
    }
}
