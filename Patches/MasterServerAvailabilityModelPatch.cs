using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using BeatTogether.Model;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MasterServerAvailabilityModel), "GetAvailabilityAsyncInternal")]
    class GetAvailabilityAsyncInternalPatch
    {
        internal static void Postfix(ref Task<MasterServerAvailabilityData> __result)
        {
            Plugin.Logger.Warn($"Faking server availability data.");
            __result = Task<MasterServerAvailabilityData>.Run(() => {
                (new ServerStatusFetcher(Plugin.ServerProvider.Servers, Plugin.StatusProvider)).FetchAll();
                return new MasterServerAvailabilityData()
                {
                    minimumAppVersion = Application.version,
                    status = MasterServerAvailabilityData.AvailabilityStatus.Online
                };
            });
        }
    }
}
