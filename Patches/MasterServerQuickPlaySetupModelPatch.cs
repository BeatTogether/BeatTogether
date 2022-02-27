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
    [HarmonyPatch(typeof(QuickPlaySetupModel), "Init")]
    class MasterServerQuickPlaySetupModelInitPatch
    {
        internal static void Postfix(QuickPlaySetupModel __instance)
        {
            GameClassInstanceProvider.Instance.MasterServerQuickPlaySetupModel = __instance;
        }
    }

    // TODO: Should probably check if the server was switched or not, for now we just always return false
    [HarmonyPatch(typeof(QuickPlaySetupModel), "IsQuickPlaySetupTaskValid")]
    class IsQuickPlaySetupTaskValidPatch
    {
        internal static void Postfix(QuickPlaySetupModel __instance, ref bool __result, ref Task<QuickPlaySetupModel> ____request)
        {
            ____request = null;
            __result = false;
        }
    }

    [HarmonyPatch(typeof(QuickPlaySetupModel), "GetQuickPlaySetupAsync")]
    class GetQuickPlaySetupAsyncPatch
    {
        internal static void Postfix(QuickPlaySetupModel __instance, ref Task<QuickPlaySetupData> __result, ref Task<QuickPlaySetupModel> ____request)
        {
            if (ModStatusProvider.ShouldBlockSongPackOverrides)
            {
                __result = new Task<QuickPlaySetupData>(null);
            }

        }
    }

}
