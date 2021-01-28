using System.Linq;
using BeatTogether.Providers;
using BeatTogether.UI;
using HarmonyLib;

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
            var servers = Plugin.ServerDetailProvider.Servers;
            var serverSelection = UIFactory.CreateServerSelectionView(__instance);

            serverSelection.values = servers.ToList<object>();
            serverSelection.Value = Plugin.ServerDetailProvider.Selection;
        }
    }
}
