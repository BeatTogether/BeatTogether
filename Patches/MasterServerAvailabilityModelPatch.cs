using System.Threading.Tasks;
using BeatTogether.Providers;
using HarmonyLib;
using UnityEngine;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MasterServerAvailabilityModel), "GetAvailabilityAsyncInternal")]
    class GetAvailabilityAsyncInternalPatch
    {
        internal static void Postfix(ref Task<MasterServerAvailabilityData> __result)
        {
            __result = Task.Run(async () =>
            {
                var serverStatusFetcher = new ServerStatusFetcher(Plugin.ServerDetailProvider.Servers, Plugin.StatusProvider);
                await serverStatusFetcher.FetchAll();
                return new MasterServerAvailabilityData()
                {
                    minimumAppVersion = Application.version,
                    status = MasterServerAvailabilityData.AvailabilityStatus.Online
                };
            });
        }
    }
}
