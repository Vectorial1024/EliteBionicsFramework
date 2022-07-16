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
        public static void PostFix(Hediff hediff)
        {
            if (hediff.Part != null)
            {
                MaxHealthCache.ResetCacheSpecifically(hediff.Part);
            }
        }
    }
}
