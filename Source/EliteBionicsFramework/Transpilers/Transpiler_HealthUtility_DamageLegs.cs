using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EliteBionicsFramework.Transpilations
{
    [HarmonyPatch(typeof(HealthUtility))]
    [HarmonyPatch("DamageLegsUntilIncapableOfMoving")]
    public static class Transpiler_HealthUtility_DamageLegs
    {
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
                        yield return new CodeInstruction(OpCodes.Ldloc_3);
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
