using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations
{
    [HarmonyPatch(typeof(HealthUtility))]
    [HarmonyPatch("GiveRandomSurgeryInjuries")]
    public static class Transpiler_HealthUtility_SurgeryInjuries
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * A total of 3 GetMaxHealth occurences detected;
             * All 3 occurences are using the same code.
             * 
             * Patch the 1st one at 4th occurence
             * Patch the 2nd one at 6th occurence
             * Patch the 3rd one at 7th occurence
             */
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;
            FileLog.Log("Analyzing nested types under HealthUtility...");
            Type typeSelfAnon = null;
            foreach (Type type in typeof(HealthUtility).GetNestedTypes(BindingFlags.NonPublic))
            {
                FileLog.Log("Type is " + type.Name);
                if (type.Name.Contains("<GiveRandomSurgeryInjuries>c__AnonStorey1"))
                {
                    typeSelfAnon = type;
                    // break;
                }
            }

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 4 || occurencesCallvirt == 6 || occurencesCallvirt == 7)
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeSelfAnon, "brain"));
                        yield return new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth"));

                        suppressCount = 1;

                        if (occurencesCallvirt == 7)
                        {
                            patchComplete = true;
                        }
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
