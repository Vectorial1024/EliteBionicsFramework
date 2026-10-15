using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(Pawn_AgeTracker))]
    [HarmonyPatch(nameof(Pawn_AgeTracker.PostResolveLifeStageChange), MethodType.Normal)]
    public class PostFix_AgeTracker_PostResolveLifeStageChange
    {
        [HarmonyPostfix]
        public static void PostFix(Pawn ___pawn)
        {
            // max HP depends on body size, which then depends on the pawn's life stage
            // this listens for life stage changes, so that we can recalculate the cached max HP.
            MaxHealthCache.ResetCacheForPawn(___pawn);
            ToolPowerInfoCache.ResetCacheForPawn(___pawn);
        }
    }
}
