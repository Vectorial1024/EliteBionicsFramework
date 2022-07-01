using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EBF.Patches.Unification.Pawnmorpher
{
    [HarmonyPatch]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class Prefix_Pawnmorpher_GetPartConditionLabel
    {
        public static bool Prepare()
        {
            return ModDetector.PawnmorpherIsLoaded && false;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.HPatches.HealthUtilityPatchs.GetPartConditionLabel:Transpiler");
        }

        [HarmonyPrefix]
        public static bool PreFix(ref IEnumerable<CodeInstruction> __result, IEnumerable<CodeInstruction> insts)
        {
            // dont let pawnmorpher do it; we will read their values and print them out by ourselves
            __result = insts;
            return false;
        }
    }
}
