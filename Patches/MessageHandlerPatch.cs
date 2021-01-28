using BeatTogether.Providers;
using HarmonyLib;
using MasterServer;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MessageHandler), "Dispose")]
    internal class DisposePatch
    {
        internal static void Prefix(MessageHandler __instance)
        {
            if (GameClassInstanceProvider.Instance.UserMessageHandler != __instance)
                return;

            GameClassInstanceProvider.Instance.UserMessageHandler = null;
        }
    }
}
