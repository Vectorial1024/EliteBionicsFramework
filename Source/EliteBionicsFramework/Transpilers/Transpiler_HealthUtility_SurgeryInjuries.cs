using HarmonyLib;
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
            // FileLog.Log("Analyzing nested types under HealthUtility...");
            /*
             * This is to patch for "brain def max health" in the function GiveRandomSurgeryInjuries(...).
             * From what we know, RW generates a lot of nested compiler-generated class,
             * and among them, there is one under HealthUtility that stores info related to "brain def max health".
             * 
             * This generated class may have different names, but from what we know, this class:
             * - has a field named "brain"
             * - such field has type BodyPartRecord (this should be less important, since we can't have two fields with the same name but with different types)
             * 
             * I am assuming there is only one such class among all the nested classes under HealthUtility.
             */
            /*
             * Credits to Neceros on GitHub for pointing out this bug at the code base,
             * and credits to Bar0th (I think they are on Steam) for suggesting the following approach of finding the currect self-anon type.
             * (Back in B18 and B19, these generated nested types were usually named like "_AnonStorey" so I referred to them as self-anon types)
             */
            Type typeSelfAnon = typeof(HealthUtility).GetNestedTypes(BindingFlags.NonPublic).Where((Type type) => type.GetField("brain") != null).First();
            if (typeSelfAnon != null)
            {
                // Successfully found the anon type
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
                yield break;
            }
            else
            {
                // In the unlikely case where the search failed, change nothing and return.
                foreach (CodeInstruction instruction in instructions)
                {
                    yield return instruction;
                }
                yield break;
            }
        }
    }
}
