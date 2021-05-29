using System;
using BeatTogether.Providers;
using HarmonyLib;
using IPA.Utilities;
using MasterServer;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MasterServerConnectionManager), "InitMasterServerHandler")]
    internal class InitMasterServerHandlerPatch
    {
        internal static void Postfix(MasterServerConnectionManager __instance)
        {
            GameClassInstanceProvider.Instance.UserMessageHandler =
                ReflectionUtil.GetField<UserMessageHandler, MasterServerConnectionManager>(__instance,
                    "_messageHandler");
        }
    }
}