using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(Pawn_AgeTracker))]
    [HarmonyPatch("PostResolveLifeStageChange", MethodType.Normal)]
    public class PostFix_AgeTracker_PostResolveLifeStageChange
    {
        [HarmonyPostfix]
        public static void PostFix(HediffSet __instance)
        {
            // max HP depends on body size, which then depends on the pawn's life stage
            // this listens for life stage changes, so that we can recalculate the cached max HP.
            MaxHealthCache.ResetCacheForPawn(__instance.pawn);
        }
    }
}
