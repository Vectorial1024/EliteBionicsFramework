using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations
{
    [HarmonyPatch(typeof(Hediff_Pregnant))]
    [HarmonyPatch("IsSeverelyWounded", MethodType.Getter)]
    public static class Transpiler_HediffPregnant_SeverelyWounded
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * A total of 1 GetMaxHealth occurences detected;
             * Patch at 11th occurence
             */
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 11)
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_3);
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                        yield return new CodeInstruction(OpCodes.Callvirt, typeof(List<Hediff_MissingPart>).GetProperty("Item").GetGetMethod());
                        yield return new CodeInstruction(OpCodes.Callvirt, typeof(Hediff).GetProperty("Part").GetGetMethod());
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
