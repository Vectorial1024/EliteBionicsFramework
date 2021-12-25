using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations.PrepareCarefully
{
    [HarmonyPatch]
    public static class Transpiler_PanelHealth_GetTooltipForPart
    {
        public static bool Prepare()
        {
            return ModDetector.PrepareCarefullyIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("EdB.PrepareCarefully.PanelHealth:GetTooltipForPart");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Patch things up at the 4th occurence of callvirt
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
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
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
