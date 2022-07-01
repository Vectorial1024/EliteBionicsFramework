using EBF.Util;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(HealthCardUtility))]
    [HarmonyPatch("GetTooltip")]
    public static class Transpiler_HealthCardUtility
    {
        public static bool Prepare()
        {
            // it turns out Pawnmorpher also overrides the function, so we need to plain against that
            // the idea is to let pawnmorpher take control of a few functions, but they do so at the cost of letting us tell them the body part max HP
            // we then look at Pawnmorpher values and add our values on top of them
            return !ModDetector.PawnmorpherIsLoaded;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // StreamWriter writer = new StreamWriter(new FileStream("C:\\Users\\Vincent Wong\\Desktop\\output.txt", FileMode.Create));
            bool patchIsComplete = false;
            short occurencesCallvirt = 0;
            short suppressCount = 0;

            foreach (CodeInstruction instruction in instructions)
            {
                // Count for the 5th callvirt, convert it to a call, and suppress the original callvirt
                if (!patchIsComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 4)
                    {
                        /*
                        writer.WriteLine("Patching!");
                        writer.WriteLine(new CodeInstruction(OpCodes.Ldarg_2));
                        writer.WriteLine(new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth")));
                        */
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth"));
                        suppressCount = 1;
                        patchIsComplete = true;
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

            //writer.Close();
        }
    }
}
