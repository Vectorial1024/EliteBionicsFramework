using EBF.Util;
using HarmonyLib;
using RimWorld;

namespace EBF.Patches.Unification.CONN
{
    [HarmonyPatch(typeof(StatDef))]
    [HarmonyPatch("CanShowWithLoadedMods")]
    public class PreFix_CONN_HealthStat
    {
        public static bool Prepare()
        {
            return ModDetector.CONNIsLoaded;
        }

        [HarmonyPrefix]
        public static bool DoNotDisplayTheStat(StatDef __instance, ref bool __result)
        {
            // this normally means to display when another mod is loaded
            // but we reverse it to become hide it when another mod is laoded (which is EBF)
            if (__instance?.defName == "CONN_HealthPointAddedToPart")
            {
                // do not display this ever
                __result = false;
                return false;
            }
            return true;
        }
    }
}
