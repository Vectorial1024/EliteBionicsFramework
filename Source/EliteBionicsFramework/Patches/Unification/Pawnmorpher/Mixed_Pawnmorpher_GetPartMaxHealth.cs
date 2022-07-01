using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Patches.Unification.Pawnmorpher
{
    [HarmonyPatch]
    public class Mixed_Pawnmorpher_GetPartMaxHealth
    {
        public static bool Prepare()
        {
            return ModDetector.PawnmorpherIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.BodyUtilities:GetPartMaxHealth");
        }

        [HarmonyReversePatch]
        public static float GetPartMaxHealthDueToPawnmorpher(BodyPartRecord record, Pawn p)
        {
            // this gets the original Pawnmorpher GetPartMaxHealth...
            throw new NotImplementedException("Called a stub before reverse patching is complete.");
        }

        [HarmonyPrefix]
        public static bool ManuallyPatchTheMethod(ref float __result, BodyPartRecord record, Pawn p)
        {
            // ...so that in this method, we can reference the original method + apply a few of our own logic
            __result = EBFEndpoints.GetMaxHealthWithEBF(record, p);
            return false;
        }
    }
}
