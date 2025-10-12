using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations.IntegratedGenes
{
    [HarmonyPatch]
    public static class Transpiler_GenesFleshbeast_CanRegeneratePart
    {
        public static bool Prepare()
        {
            return ModDetector.IntegratedGenesIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("IntegratedGenes.Gene_FleshbeastRegenerating:CanRegeneratePart");
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
                    new CodeInstruction(OpCodes.Call, VanillaExtender.ReflectionGetMaxHealth())
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                .Set(OpCodes.Nop, null) // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
