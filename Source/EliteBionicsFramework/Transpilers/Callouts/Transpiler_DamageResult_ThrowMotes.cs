using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations.Callouts
{
    [HarmonyPatch]
    public static class Transpiler_DamageResult_ThrowMotes
    {
        public static bool Prepare()
        {
            // dont do this patch if Pawnmorpher is detected; there are race conditions
            // if Pawnmorpher is loaded then we use PostFix_Pawnmorpher_HealthUtil instead.
            return ModDetector.CalloutsIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("CM_Callouts.DamageWorker_DamageResult_Patches+DamageWorker_DamageResult_AssociateWithLog:ThrowDestroyedPartMotes");
        }

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
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                        yield return new CodeInstruction(OpCodes.Callvirt, typeof(List<BodyPartRecord>).GetProperty("Item").GetGetMethod());
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
