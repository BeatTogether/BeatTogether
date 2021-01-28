using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities;
using BeatTogether.Model;
using MasterServer;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(UserMessageHandler), MethodType.Constructor,
        new Type[] { typeof(MessageHandler.IMessageSender), typeof(PacketEncryptionLayer), typeof(MasterServerEndPoint), typeof(IAuthenticationTokenProvider) })]
    internal class BaseClientMessageHandlerCtorPatch
    {
        internal static void Postfix(UserMessageHandler __instance,
            MessageHandler.IMessageSender sender,
            PacketEncryptionLayer encryptionLayer,
            MasterServerEndPoint endPoint,
            IAuthenticationTokenProvider authenticationTokenProvider)
        {
            Plugin.Logger.Debug("UserMessageHandler is going to be replaced.");
            GameClassInstanceProvider.Instance.UserMessageHandler = __instance;
        }
    }

    [HarmonyPatch(typeof(MessageHandler), "Dispose")]
    internal class BaseClientMessageHandlerDisposePatch
    {
        internal static void Prefix(MessageHandler __instance)
        {
            if (GameClassInstanceProvider.Instance.UserMessageHandler != __instance)
            {
                return;
            }

            Plugin.Logger.Debug("UserMessageHandler is going to be releted.");
            GameClassInstanceProvider.Instance.UserMessageHandler = null;
        }
    }
}
