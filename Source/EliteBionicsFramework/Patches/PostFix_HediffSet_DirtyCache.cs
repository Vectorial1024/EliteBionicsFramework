using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch("DirtyCache", MethodType.Normal)]
    public class PostFix_HediffSet_DirtyCache
    {
        [ThreadStatic]
        internal static HashSet<Pawn> dirtyCacheIgnoreSet = new HashSet<Pawn>();

        /// <summary>
        /// Suppresses the next "EBF Protocol violation" error message.
        /// Because RimWorld is only single-threaded, we can do crazy things like this.
        /// </summary>
        internal static void SuppressNextDirtyCache(Pawn target)
        {
            dirtyCacheIgnoreSet.Add(target);
        }

        [HarmonyPostfix]
        public static void PostFix(HediffSet __instance)
        {
            // convenient place to keep track of hediff/alive changes
            Pawn pawn = __instance.pawn;
            if (dirtyCacheIgnoreSet.Contains(pawn))
            {
                // but dont do it if we are told to ignore it
                dirtyCacheIgnoreSet.Remove(pawn);
                return;
            }
            MaxHealthCache.ResetCacheForPawn(pawn);
        }
    }
}
