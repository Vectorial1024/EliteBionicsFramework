using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(HealthUtility))]
    [HarmonyPatch(nameof(HealthUtility.GiveRandomSurgeryInjuries))]
    public static class Transpiler_HealthUtility_SurgeryInjuries
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * A total of 3 GetMaxHealth occurences detected;
             * Patch with CodeMatcher
             * Fortunately all occurences have the same code patch
             */

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
            if (typeSelfAnon == null)
            {
                // In the unlikely case where the search failed, change nothing and return.
                EliteBionicsFrameworkMain.LogError("Patch failed: surgery injuries, failed to find relevant self-anon type!");
                return instructions;
            }
            // Successfully found the anon type
            return new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth)))
                ) // find the patterns of .GetMaxHealth()
                .Repeat(delegate(CodeMatcher matcher)
                {
                    matcher.InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeSelfAnon, "brain")),
                        new CodeInstruction(OpCodes.Call, VanillaExtender.ReflectionGetMaxHealth())
                    ); // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                    matcher.Set(OpCodes.Nop, null); // and ignore the original instruction
                }) // repeat for all matches
                .InstructionEnumeration();
        }
    }
}
