using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations.Moody
{
    [HarmonyPatch]
    public static class Transpiler_Patches_GetPartConditionLabel
    {
        public static bool Prepare()
        {
            return ModDetector.PawnQuickInfoIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("PawnQuickInfo.Patches:_GetPartConditionLabel");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * A total of 1 GetMaxHealth occurences detected;
             * Patch with CodeMatcher
             */
            return new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth)))
                ) // find the only occurence of .GetMaxHealth()
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Hediff).GetProperty("Part").GetGetMethod()),
                    new CodeInstruction(OpCodes.Call, VanillaExtender.ReflectionGetMaxHealth_Cached())
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth_Cached(); we do this out of convenience
                .Set(OpCodes.Nop, null) // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
