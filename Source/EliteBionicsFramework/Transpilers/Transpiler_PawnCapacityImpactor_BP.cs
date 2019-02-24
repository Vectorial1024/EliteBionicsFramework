using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EliteBionicsFramework.Transpilations
{
    // This does not seem necessary.
    [HarmonyPatch(typeof(PawnCapacityUtility.CapacityImpactorBodyPartHealth))]
    [HarmonyPatch("Readable", MethodType.Normal)]
    public static class Transpiler_PawnCapacityImpactor_BP
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * A total of 1 GetMaxHealth occurences detected;
             * Patch at 3rd occurence
             */
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 3)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PawnCapacityUtility.CapacityImpactorBodyPartHealth), "bodyPart"));
                        yield return new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth")); ;

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
