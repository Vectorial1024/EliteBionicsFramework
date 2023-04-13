using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch("DirtyCache", MethodType.Normal)]
    public class PostFix_HediffSet_DirtyCache
    {
        [HarmonyPostfix]
        public static void PostFix(HediffSet __instance)
        {
            // convenient place to keep track of hediff/alive changes
            MaxHealthCache.ResetCacheForPawn(__instance.pawn);
        }
    }
}
