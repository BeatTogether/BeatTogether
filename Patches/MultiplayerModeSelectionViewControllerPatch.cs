using HarmonyLib;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MultiplayerModeSelectionViewController), "DidActivate")]
    internal class DidActivatePatch2
    {
        internal static bool Prefix(MultiplayerModeSelectionViewController __instance, bool firstActivation)
        {
            if (!Plugin.Configuration.Enabled || !firstActivation)
                return true;

            var transform = __instance.gameObject.transform;
            var quickPlayButton = transform.Find("Buttons/QuickPlayButton").gameObject;
            quickPlayButton.SetActive(false);
            return true;
        }
    }
}
