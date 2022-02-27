using BeatTogether.Models;
using HarmonyLib;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(NetworkConfigSO), "multiplayerStatusUrl", MethodType.Getter)]
    internal class GetMasterServerStatusUrlPatch
    {
        [HarmonyBefore("mod.serverbrowser")]
        internal static void Postfix(ref string __result)
        {
            ServerDetails.OfficialStatusUri = __result;

            var server = Plugin.ServerDetailProvider.Selection;
            if (server.IsOfficial)
                return;

            __result = server.StatusUri;
            Plugin.Logger.Debug($"Patching master server status URL (URL='{__result}').");
        }
    }

    [HarmonyPatch(typeof(NetworkConfigSO), "masterServerEndPoint", MethodType.Getter)]
    internal class GetMasterServerEndPointPatch
    {
        [HarmonyBefore("mod.serverbrowser")]
        internal static void Postfix(ref DnsEndPoint __result)
        {
            var server = Plugin.ServerDetailProvider.Selection;
            if (server.IsOfficial)
                return;

            __result = server.GetEndPoint();
            Plugin.Logger.Debug($"Patching master server end point (EndPoint='{__result}').");
        }
    }
}
