using HarmonyLib;
using Verse;

namespace EBF.Patches
{

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch(nameof(Pawn_HealthTracker.Notify_HediffChanged), MethodType.Normal)]
    public class Prefix_HealthTracker_NotifyHediffChanged
    {
        [HarmonyPrefix]
        public static void PreFix(Pawn ___pawn, Hediff hediff)
        {
            if (hediff is Hediff_Injury || hediff is Hediff_MissingPart)
            {
                // healing injuries will very often tick the health cache, but it is irrelevant to our operations
                // therefore, ignore the next dirty cache call

                // missing part hediffs may tick after 90k ticks, but ultimately they are irrelevant to our operations
                // if someone lost a body part, then supposedly somewhere in the codebase, the game will call this function with a null parameter or something

                // PostFix_HediffSet_DirtyCache.SuppressNextDirtyCache(___pawn);
            }
        }
    }
}
