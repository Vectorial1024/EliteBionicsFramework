using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations
{
    [HarmonyPatch(typeof(HealthUtility))]
    [HarmonyPatch(nameof(HealthUtility.GetPartConditionLabel))]
    public static class Transpiler_HealthUtility_PartConditionLabel
    {
        /*
         * Keeping this here until Harmony Library becomes stable again.
         * Note: Chaos ensued after Harmony failed to patch some RimWorld methods that returns structs.
         * This patch file is dealing with a method tha returns structs...
        [HarmonyPrefix]
        public static bool PreFix(Pawn pawn, BodyPartRecord part)
        {
            EliteBionicsFrameworkMain.LogError("Pawn, part: " + pawn.Name + ", " + part.def.defName);
            return true;
        }
        */

        public static bool Prepare()
        {
            // dont do it if Pawnmorpher is laoded; apparently they are doing exactly the same thing, and our decision is to embed our logic to their code via patches
            return !ModDetector.PawnmorpherIsLoaded;
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
                    // Optimize yeah
                    // special note: this might create confusion because this is the value being displayed, which may be different from the real value for a short time due to the cache
                    new CodeInstruction(OpCodes.Call, VanillaExtender.ReflectionGetMaxHealth_Cached())
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                .Set(OpCodes.Nop, null) // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
