using EBF.Util;
using HarmonyLib;
using System.Reflection;
using Verse;

namespace EBF.Patches.Unification.Pawnmorpher
{
    [HarmonyPatch]
    public class PostFix_Pawnmorpher_HediffStageChanges_Recache
    {
        public static bool Prepare()
        {
            return ModDetector.PawnmorpherIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.Hediffs.Hediff_StageChanges:RecacheStage");
        }

        [HarmonyPostfix]
        public static void NotifyRecalculateHealth(Hediff __instance)
        {
            // state changed; recalculate the health!
            MaxHealthCache.ResetCacheForPawn(__instance.pawn);
            ToolPowerInfoCache.ResetCacheForPawn(__instance.pawn);
        }
    }
}
