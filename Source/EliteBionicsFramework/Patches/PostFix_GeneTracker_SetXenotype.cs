using EBF.Util;
using HarmonyLib;
using RimWorld;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(Pawn_GeneTracker))]
    [HarmonyPatch(nameof(Pawn_GeneTracker.SetXenotype), MethodType.Normal)]
    public class PostFix_GeneTracker_SetXenotype
    {
        [HarmonyPostfix]
        public static void ResetCacheForPawn(Pawn ___pawn)
        {
            // xenotypes affect the entire pawn, and are somewhat similar to whole-body hediffs
            MaxHealthCache.ResetCacheForPawn(___pawn);
            ToolPowerInfoCache.ResetCacheForPawn(___pawn);
        }
    }
}
