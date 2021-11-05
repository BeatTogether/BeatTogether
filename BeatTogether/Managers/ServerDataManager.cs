using BeatTogether.Providers;
using SiraUtil.Affinity;
using SiraUtil.Logging;

namespace BeatTogether.Managers
{
    public class ServerDataManager : IAffinity
    {
        private readonly ServerDetailProvider _serverDetails;
        private readonly SiraLog _logger;

        internal ServerDataManager(ServerDetailProvider serverDetails, SiraLog logger)
        {
            _serverDetails = serverDetails;
            _logger = logger;
        }

        [AffinityPostfix]
        [AffinityBefore("mod.serverbrowser")]
        [AffinityPatch(typeof(NetworkConfigSO), nameof(NetworkConfigSO.masterServerEndPoint), AffinityMethodType.Getter)]
        private void GetMasterServerEndPoint(ref MasterServerEndPoint __result)
        {
            var server = _serverDetails.SelectedServer;
            if (server.IsOfficial || server.EndPoint == null)
                return;

            __result = server.EndPoint;
            _logger.Debug($"Patching master server end point (EndPoint='{__result}').");
        }

        [AffinityPostfix]
        [AffinityBefore("mod.serverbrowser")]
        [AffinityPatch(typeof(NetworkConfigSO), nameof(NetworkConfigSO.masterServerStatusUrl), AffinityMethodType.Getter)]
        private void GetMasterServerStatusUrl(ref string __result)
        {
            var server = _serverDetails.SelectedServer;
            if (server.IsOfficial || server.StatusUri == null)
                return;

            __result = server.StatusUri;
            _logger.Debug($"Patching master server status URL (URL='{__result}').");
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MultiplayerUnavailableReasonMethods), nameof(MultiplayerUnavailableReasonMethods.TryGetMultiplayerUnavailableReason))]
        private bool TryGetMultiplayerUnavailableReason(ref bool __result)
        {
            __result = false;
            return false;
        }

        [AffinityPatch(typeof(UserCertificateValidator), "ValidateCertificateChainInternal")]
        internal bool ValidateCertificateChain()
        {
            var server = _serverDetails.SelectedServer;
            if (server.IsOfficial)
                return true;

            // It'd be best if we do certificate validation here...
            // but for now we'll just skip it.
            return false;
        }
    }
}
