using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MasterServerAvailabilityModel), "GetAvailabilityAsyncInternal")]
    class GetAvailabilityAsyncInternalPatch
    {
        private static readonly string FAKE_RESPONSE_TEMPLATE = "{\"minimumAppVersion\":\"1.13.0\",\"status\":0,\"maintenanceStartTime\":0,\"maintenanceEndTime\":0,\"userMessage\":{\"localizedMessages\":[]}}";

        internal static void Postfix(ref Task<MasterServerAvailabilityData> __result)
        {
            Plugin.Logger.Warn($"Faking server availability data.");
            __result = Task<MasterServerAvailabilityData>.Run(() => {
                return JsonUtility.FromJson<MasterServerAvailabilityData>(FAKE_RESPONSE_TEMPLATE);
            });
        }
    }
}
