using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(DamageWorker_AddInjury))]
    [HarmonyPatch("ReduceDamageToPreserveOutsideParts")]
    public static class Transpiler_DamageWorker_AddInjury
    {
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
                    new CodeInstruction(OpCodes.Ldarga_S, 2),
                    new CodeInstruction(OpCodes.Call, typeof(DamageInfo).GetProperty("HitPart").GetGetMethod()),
                    new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth"))
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                .Set(OpCodes.Nop, null) // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
