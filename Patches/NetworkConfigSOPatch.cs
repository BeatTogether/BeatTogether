using HarmonyLib;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(NetworkConfigSO), "masterServerStatusUrl", MethodType.Getter)]
    internal class GetMasterServerStatusUrlPatch
    {
        internal static bool Prefix(ref string __result)
        {
            if (!Plugin.Configuration.Enabled)
                return true;

            __result = Plugin.Configuration.StatusUrl;
            Plugin.Logger.Info($"Patching master server status URL (URL='{__result}').");
            return false;
        }
    }

    [HarmonyPatch(typeof(NetworkConfigSO), "masterServerEndPoint", MethodType.Getter)]
    internal class GetMasterServerEndPointPatch
    {
        internal static bool Prefix(ref MasterServerEndPoint __result)
        {
            if (!Plugin.Configuration.Enabled)
                return true;

            __result = new MasterServerEndPoint(Plugin.Configuration.HostName, Plugin.Configuration.Port);
            Plugin.Logger.Debug($"Patching master server end point (EndPoint='{__result}').");
            return false;
        }
    }
}
