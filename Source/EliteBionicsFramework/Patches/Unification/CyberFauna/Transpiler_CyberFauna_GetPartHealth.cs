using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EBF.Patches.Unification.CyberFauna
{
    [HarmonyPatch]
    public class Transpiler_CyberFauna_GetPartHealth
    {
        public static bool Prepare()
        {
            return ModDetector.CyberFaunaIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("ProthesisHealth.HediffSet_GetPartHealth_ParagonPatch:Prefix");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // overwrite the thing to be "return true"
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Ret);
        }
    }
}
