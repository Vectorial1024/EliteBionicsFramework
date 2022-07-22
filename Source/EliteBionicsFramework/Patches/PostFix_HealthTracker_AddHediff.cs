using EBF;
using EBF.Util;
using HarmonyLib;
using System;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("AddHediff", MethodType.Normal)]
    [HarmonyPatch(new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
    public class PostFix_HealthTracker_AddHediff
    {
        [HarmonyPostfix]
        public static void PostFix(Pawn ___pawn, BodyPartRecord part)
        {
            MaxHealthCache.ResetCacheSpecifically(___pawn, part);
        }
    }
}
