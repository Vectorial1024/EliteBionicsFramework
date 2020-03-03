using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations
{
    [HarmonyPatch(typeof(HealthUtility))]
    [HarmonyPatch("GetPartConditionLabel")]
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

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Patch things up at the 2nd occurence of callvirt
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 2)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth"));

                        suppressCount = 1;
                        patchComplete = true;
                    }
                }

                if (suppressCount > 0)
                {
                    instruction.opcode = OpCodes.Nop;
                    suppressCount--;
                }

                yield return instruction;
            }
        }
    }
}
