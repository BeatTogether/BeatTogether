using System;
using BeatTogether.Providers;
using HarmonyLib;
using MasterServer;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(UserMessageHandler), MethodType.Constructor,
        new Type[] { typeof(MessageHandler.IMessageSender), typeof(PacketEncryptionLayer), typeof(MasterServerEndPoint), typeof(IAuthenticationTokenProvider) })]
    internal class CtorPatch
    {
        internal static void Postfix(UserMessageHandler __instance,
            MessageHandler.IMessageSender sender,
            PacketEncryptionLayer encryptionLayer,
            MasterServerEndPoint endPoint,
            IAuthenticationTokenProvider authenticationTokenProvider)
        {
            GameClassInstanceProvider.Instance.UserMessageHandler = __instance;
        }
    }
}
