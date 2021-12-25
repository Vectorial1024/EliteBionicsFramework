using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations.Moody
{
    [HarmonyPatch]
    public static class Transpiler_HealthCardUtility_GetTooltip
    {
        public static bool Prepare()
        {
            // dont do this patch if Pawnmorpher is detected; there are race conditions
            // if Pawnmorpher is loaded then we use PostFix_Pawnmorpher_HealthUtil instead.
            return ModDetector.MoodyIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Moody.Extensions.RimWorld.HealthCardUtility:GetTooltip");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Patch things up at the 13th occurence of callvirt
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 13)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_3);
                        yield return new CodeInstruction(OpCodes.Callvirt, typeof(Hediff).GetProperty("Part").GetGetMethod());
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
