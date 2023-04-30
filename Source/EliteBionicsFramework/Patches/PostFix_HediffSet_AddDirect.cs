using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch("AddDirect", MethodType.Normal)]
    public class PostFix_HediffSet_AddDirect
    {
        [HarmonyPostfix]
        public static void ResetCacheSpecifically(Pawn ___pawn, Hediff hediff)
        {
            if (hediff.Part != null)
            {
                return;
            }
            MaxHealthCache.ResetCacheSpecifically(___pawn, hediff.Part);
        }
    }
}
