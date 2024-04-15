using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch(nameof(HediffSet.AddDirect), MethodType.Normal)]
    public class PostFix_HediffSet_AddDirect
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
        }
    }
}
