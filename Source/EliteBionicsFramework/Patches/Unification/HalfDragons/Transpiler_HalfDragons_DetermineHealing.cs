using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Patches.Unification.HalfDragons
{
    [HarmonyPatch]
    public class Transpiler_HalfDragons_DetermineHealing
    {
        public static bool Prepare()
        {
            return TargetMethod() != null;
        }

        public static MethodBase TargetMethod()
        {
            if (!ModDetector.HalfDragonsIsLoaded)
            {
                return null;
            }
            /*
             * Oh no, the terrors of self-anon...
             * Refer to documentation elsewhere for an explanation of self-anon.
             * Just search "selfanon" and you should get something.
             * 
             * This time, wa are patching a method inside the self-anon class.
             * This reminds me of a certain RimWorld mod that I have abandoned because it involves patching some values inside self-anon types...
             */
            Type typeSelfAnon = AccessTools.TypeByName("HalfDragons.Need_DragonBlood").GetNestedTypes(BindingFlags.NonPublic).Where((Type type) => type.GetField("hediffs") != null).First();
            if (typeSelfAnon == null)
            {
                return null;
            }
            // it should exist, but it has no known name
            // match the signature of function (BodyPartRecord)
            MethodInfo theMethod = null;
            foreach (MethodInfo method in typeSelfAnon.GetMethods())
            {
                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    // it should only have 1 param
                    if (parameter.ParameterType == typeof(BodyPartRecord))
                    {
                        // is this one
                        theMethod = method;
                    }
                    break;
                }
                if (theMethod != null)
                {
                    break;
                }
            }
            // we should have matched the method...
            return theMethod;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Patch things up at the 2nd occurence of callvirt
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            // we are here only if we have found the method
            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 2)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth_Cached"));

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
