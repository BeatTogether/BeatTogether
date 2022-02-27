using BeatTogether.Providers;
using HarmonyLib;
using MasterServer;

namespace BeatTogether.Patches
{
    //[HarmonyPatch(typeof(UserMasterServerMessageHandler), "Dispose")]
    //internal class DisposePatch
    //{
    //    internal static void Prefix(UserMasterServerMessageHandler __instance)
    //    {
    //        if (GameClassInstanceProvider.Instance.UserMessageHandler != __instance)
    //            return;

    //        GameClassInstanceProvider.Instance.UserMessageHandler = null;
    //    }
    //}
}
