using HarmonyLib;
using MasterServer;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(UserCertificateValidator), "ValidateCertificateChainInternal")]
    internal class ValidateCertificateChainInternalPatch
    {
        internal static bool Prefix()
        {
            var server = Plugin.ServerDetailProvider.Selection;
            if (server.IsOfficial)
                return true;

            // It'd be best if we do certificate validation here...
            // but for now we'll just skip it.
            return false;
        }
    }
}
