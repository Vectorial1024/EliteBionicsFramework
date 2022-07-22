using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("RemoveHediff", MethodType.Normal)]
    public class PostFix_HealthTracker_RemoveHediff
    {
        [HarmonyPostfix]
        public static void PostFix(Pawn ___pawn, Hediff hediff)
        {
            if (hediff.Part != null)
            {
                MaxHealthCache.ResetCacheSpecifically(___pawn, hediff.Part);
            }
        }
    }
}
