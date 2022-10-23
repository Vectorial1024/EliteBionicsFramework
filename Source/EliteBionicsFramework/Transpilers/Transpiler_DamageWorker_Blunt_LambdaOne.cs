using EBF.Util;
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
    [HarmonyPatch]
    public static class Transpiler_DamageWorker_Blunt_LambdaOne
    {
        public static bool Prepare()
        {
            bool available = TargetMethod() != null;
            if (!available)
            {
                EliteBionicsFrameworkMain.LogError("Failed to detect target self-anon type for patching: blunt lambda one");
            }
            return available;
        }

        public static MethodBase TargetMethod()
        {
            // (we will let this print error when it fails to detect things... somehow we cannot properly encapsulate the error correctly)

            // RW v1.4, FQ name Verse.DamageWorker_Blunt+<>c.<StunChances>b__2_0
            // weak matching: this self-anon type has 9 fields
            var potentialSelfAnons = AccessTools.TypeByName("Verse.DamageWorker_Blunt").GetNestedTypes(BindingFlags.NonPublic).Where((Type type) => type.GetFields().Length == 9);
            if (potentialSelfAnons.Count() == 0)
            {
                EliteBionicsFrameworkMain.LogError("field-count criteria fail");
                return null;
            }
            Type typeSelfAnon = potentialSelfAnons.First();
            // and then, within these 9 fields, pick the one with signature (ThingDef, float32, bool) -> string
            foreach (MethodInfo info in typeSelfAnon.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                // weak match: this is the only method that has this many parameters
                if (info.GetParameters().Length == 3)
                {
                    // is this one!
                    return info;
                }
            }
            // could not find it
            EliteBionicsFrameworkMain.LogError("field-count criteria fit, signature check fail");
            return null;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 4)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingDef), "race"));
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RaceProperties), "body"));
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BodyDef), "corePart"));
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

                //writer.WriteLine(instruction);
                yield return instruction;
            }
        }
    }
}
