using HarmonyLib;
using BeatTogether.Configuration;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(NetworkConfigSO), "masterServerStatusUrl", MethodType.Getter)]
    internal class GetMasterServerStatusUrlPatch
    {
        [HarmonyBefore("mod.serverbrowser")]
        internal static void Postfix(ref string __result)
        {
            ServerDetails.OfficialStatusUri = __result;

            var server = Plugin.ServerProvider.Selection;
            if (server.IsOfficial)
            {
                return;
            }

            __result = server.StatusUri;
            Plugin.Logger.Info($"Patching master server status URL (URL='{__result}').");
        }
    }

    [HarmonyPatch(typeof(NetworkConfigSO), "masterServerEndPoint", MethodType.Getter)]
    internal class GetMasterServerEndPointPatch
    {
        [HarmonyBefore("mod.serverbrowser")]
        internal static void Postfix(ref MasterServerEndPoint __result)
        {
            var server = Plugin.ServerProvider.Selection;
            if (server.IsOfficial)
            {
                return;
            }

            __result = server.GetEndPoint();
            Plugin.Logger.Debug($"Patching master server end point (EndPoint='{__result}').");
        }
    }
}
