﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations
{
    // It should be possible to convert this into a standard prefix-postfix patch with high modularity.
    // TODO
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch("GetPartHealth", MethodType.Normal)]
    public static class Transpiler_HediffSet_GetPartHealth
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * A total of 1 GetMaxHealth occurences detected;
             * Patch at 1st occurence
             */
            bool patchComplete = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    // Optimize yeah
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth")); ;

                    instruction.opcode = OpCodes.Nop;
                    patchComplete = true;
                }

                yield return instruction;
            }
        }
    }
}
