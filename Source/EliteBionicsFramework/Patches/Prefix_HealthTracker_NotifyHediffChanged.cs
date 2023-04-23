using HarmonyLib;
using Verse;

namespace EBF.Patches
{

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("Notify_HediffChanged", MethodType.Normal)]
    public class Prefix_HealthTracker_NotifyHediffChanged
    {
        [HarmonyPrefix]
        public static void PreFix(Pawn ___pawn, Hediff hediff)
        {
            if (hediff is Hediff_Injury)
            {
                // healing injuries will very often tick the health cache, but it is irrelevant to our operations
                // therefore, ignore the next dirty cache call
                Log.Error("Ricking Hediff_Injury for pawn " + ___pawn.ToStringSafe() + "; skipping it.");
                PostFix_HediffSet_DirtyCache.SuppressNextDirtyCache(___pawn);
            }
        }
    }
}
