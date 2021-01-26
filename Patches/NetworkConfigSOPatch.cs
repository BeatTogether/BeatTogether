using HarmonyLib;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(NetworkConfigSO), "masterServerStatusUrl", MethodType.Getter)]
    internal class GetMasterServerStatusUrlPatch
    {
        internal static void Postfix(ref string __result)
        {
            if (Plugin.Configuration.Enabled)
            {
                __result = Plugin.Configuration.StatusUrl;
                Plugin.Logger.Info($"Patching master server status URL (URL='{__result}').");
            }
        }
    }

    [HarmonyPatch(typeof(NetworkConfigSO), "masterServerEndPoint", MethodType.Getter)]
    internal class GetMasterServerEndPointPatch
    {
        internal static void Postfix(ref MasterServerEndPoint __result)
        {
            if (Plugin.Configuration.Enabled)
            {
                __result = new MasterServerEndPoint(Plugin.Configuration.HostName, Plugin.Configuration.Port);
                Plugin.Logger.Debug($"Patching master server end point (EndPoint='{__result}').");
            }
        }
    }
}
