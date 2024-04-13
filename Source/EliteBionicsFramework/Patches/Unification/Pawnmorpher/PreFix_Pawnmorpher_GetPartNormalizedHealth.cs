using EBF.Util;
using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace EBF.Patches.Unification.Pawnmorpher
{
    [HarmonyPatch]
    public class PreFix_Pawnmorpher_GetPartNormalizedHealth
    {
        public static bool Prepare()
        {
            return ModDetector.PawnmorpherIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.BodyUtilities:GetPartNormalizedHealth");
        }

        [HarmonyPrefix]
        public static bool ManuallyPatchTheMethod(ref float __result, BodyPartRecord record, Pawn p)
        {
            // tell the relevant side to ignore the next getMaxHealth warning
            PostFix_BodyPart_GetMaxHealth.SuppressNextWarning();
            return true;
        }
    }
}
