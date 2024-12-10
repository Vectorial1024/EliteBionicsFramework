using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Verse;

namespace EBF.Transpilers.AlteredCarbon
{
    [HarmonyPatch]
    public class Transpiler_AC2_StatWorker_BodyPart
    {
        public static bool Prepare()
        {
            return ModDetector.AlteredCarbon2IsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("AlteredCarbon.Hediff_NeuralStack:SpawnStack");
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
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Hediff), "part")),
                    new CodeInstruction(OpCodes.Call, VanillaExtender.ReflectionGetMaxHealth())
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                .Set(OpCodes.Nop, null) // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
