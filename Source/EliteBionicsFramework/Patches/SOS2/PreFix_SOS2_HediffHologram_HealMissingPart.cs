using EBF.Util;
using HarmonyLib;
using System.Reflection;
using Verse;

namespace EBF.Patches.SOS2
{
    [HarmonyPatch]
    public class PreFix_SOS2_HediffHologram_HealMissingPart
    {
        public static bool Prepare()
        {
            return ModDetector.SaveOurShips2IsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("SaveOurShips2.HediffPawnIsHologram:HealMissingPart");
        }

        [HarmonyPrefix]
        public static void AllowGetMaxHealth(Pawn pawn, BodyPartRecord part, ref string __result)
        {
            // since it involves missing parts, there cannot be any other hediffs attached to it, so calling GetMaxHealth is permitted.
            // just suppress the warning, and then is ok
            PostFix_BodyPart_GetMaxHealth.SuppressNextWarning();
            return;
        }
    }
}
