using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations
{
    // It should be possible to convert this into a standard prefix-postfix patch with high modularity.
    // TODO
    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch("GetPartHealth", MethodType.Normal)]
    public static class Transpiler_HediffSet_GetPartHealth
    {
        public static bool Prepare()
        {
            // dont do this patch if Pawnmorpher is detected; there are race conditions
            // if Pawnmorpher is loaded then we use PostFix_Pawnmorpher_HealthUtil instead.
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
