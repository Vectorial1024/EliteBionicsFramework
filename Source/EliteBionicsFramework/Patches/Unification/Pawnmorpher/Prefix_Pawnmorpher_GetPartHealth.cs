using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace EBF.Patches.Unification.Pawnmorpher
{
    [HarmonyPatch]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class Prefix_Pawnmorpher_GetPartHealth
    {
        public static bool Prepare()
        {
            return ModDetector.PawnmorpherIsLoaded & false;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.HPatches.HediffSetPatches+GetPartHealthTranspiler:Transpiler");
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
