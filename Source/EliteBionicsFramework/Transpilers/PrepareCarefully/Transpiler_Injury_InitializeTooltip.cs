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
    public static class Transpiler_Injury_InitializeTooltip
    {
        public static bool Prepare()
        {
            return ModDetector.PrepareCarefullyIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("EdB.PrepareCarefully.Injury:InitializeTooltip");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Patch things up at the 8th occurence of callvirt
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 8)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        // full name is required!!!
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(AccessTools.TypeByName("EdB.PrepareCarefully.Injury"), "hediff"));
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
