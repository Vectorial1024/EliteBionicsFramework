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
        /// <summary>
        /// For null safety, please use the property with a similar name instead.
        /// </summary>
        [ThreadStatic]
        internal static HashSet<Pawn> dirtyCacheIgnoreSet = new HashSet<Pawn>();

        internal static HashSet<Pawn> DirtyCacheIgnoreSet
        {
            get
            {
                if (dirtyCacheIgnoreSet == null)
                {
                    dirtyCacheIgnoreSet = new HashSet<Pawn>();
                }
                return dirtyCacheIgnoreSet;
            }
        }

        /// <summary>
        /// Suppresses the next "hediff set cache is dirty" event.
        /// Because RimWorld is only single-threaded, we can do crazy things like this.
        /// </summary>
        internal static void SuppressNextDirtyCache(Pawn target)
        {
            DirtyCacheIgnoreSet.Add(target);
        }

        internal static void InitOrResetSuppressionMemory()
        {
            DirtyCacheIgnoreSet.Clear();
        }

        [HarmonyPostfix]
        public static void PostFix(HediffSet __instance)
        {
            // convenient place to keep track of hediff/alive changes
            Pawn pawn = __instance.pawn;
            if (DirtyCacheIgnoreSet.Contains(pawn))
            {
                // but dont do it if we are told to ignore it
                DirtyCacheIgnoreSet.Remove(pawn);
                return;
            }
            MaxHealthCache.ResetCacheForPawn(pawn);
        }
    }
}
