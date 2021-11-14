using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Loader;
using BeatTogether.Providers;

namespace BeatTogether.Patches
{
    [HarmonyPatch(typeof(MasterServerQuickPlaySetupModel), "Init")]
    class MasterServerQuickPlaySetupModelInitPatch
    {
        internal static void Postfix(MasterServerQuickPlaySetupModel __instance)
        {
            GameClassInstanceProvider.Instance.MasterServerQuickPlaySetupModel = __instance;
        }
    }

    // TODO: Should probably check if the server was switched or not, for now we just always return false
    [HarmonyPatch(typeof(MasterServerQuickPlaySetupModel), "IsQuickPlaySetupTaskValid")]
    class IsQuickPlaySetupTaskValidPatch
    {
        internal static void Postfix(MasterServerQuickPlaySetupModel __instance, ref bool __result, ref Task<MasterServerQuickPlaySetupData> ____request)
        {
            ____request = null;
            __result = false;
        }
    }

    [HarmonyPatch(typeof(MasterServerQuickPlaySetupModel), "GetQuickPlaySetupAsync")]
    class GetQuickPlaySetupAsyncPatch
    {
        internal static void Postfix(MasterServerQuickPlaySetupModel __instance, ref Task<MasterServerQuickPlaySetupData> __result, ref Task<MasterServerQuickPlaySetupData> ____request)
        {
            if (ModStatusProvider.ShouldBlockSongPackOverrides)
            {
                __result = new Task<MasterServerQuickPlaySetupData>(null);
            }

        }
    }

}
