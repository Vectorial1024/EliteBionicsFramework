using Harmony;
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
    [HarmonyPatch(typeof(HealthCardUtility))]
    [HarmonyPatch("GetTooltip")]
    public static class Transpiler_HealthCardUtility
    {
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

                    if (occurencesCallvirt == 5)
                    {
                        /*
                        writer.WriteLine("Patching!");
                        writer.WriteLine(new CodeInstruction(OpCodes.Ldarg_2));
                        writer.WriteLine(new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth")));
                        */
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
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
