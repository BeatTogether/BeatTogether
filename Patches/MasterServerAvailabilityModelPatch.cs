using System.Threading.Tasks;
using BeatTogether.Providers;
using HarmonyLib;
using UnityEngine;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MultiplayerStatusModel), "GetMultiplayerStatusAsyncInternal")]
    class GetAvailabilityAsyncInternalPatch
    {
        internal static void Postfix(ref Task<MultiplayerStatusData> __result)
        {
            __result = Task.Run(async () =>
            {
                var serverStatusFetcher = new ServerStatusFetcher(Plugin.ServerDetailProvider.Servers, Plugin.StatusProvider);
                await serverStatusFetcher.FetchAll();
                return new MultiplayerStatusData()
                {
                    minimumAppVersion = Application.version,
                    status = MultiplayerStatusData.AvailabilityStatus.Online
                };
            });
        }
    }
}
