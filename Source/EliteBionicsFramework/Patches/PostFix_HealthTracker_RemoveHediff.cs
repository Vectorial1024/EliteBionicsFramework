using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    // note: we are reverting to the add/remove dirtying method to help with perfomrnace
    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch(nameof(Pawn_HealthTracker.RemoveHediff), MethodType.Normal)]
    public class PostFix_HealthTracker_RemoveHediff
    {
        [HarmonyPostfix]
        public static void ResetCacheSpecifically(Pawn ___pawn, Hediff hediff)
        {
            if (hediff.Part == null)
            {
                // we are not interested in full-body hediffs
                return;
            }
            if (hediff is Hediff_Injury)
            {
                // normally, injuries are not supposed to influence the max HP stats of a body part, so we can safely ignore them
                return;
            }
            MaxHealthCache.ResetCacheSpecifically(___pawn, hediff.Part);
            ToolPowerInfoCache.ResetCacheForPawn(___pawn);
        }
    }
}
