using Harmony;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace EliteBionicsFramework.Transpilations
{
    [HarmonyPatch(typeof(DamageWorker_AddInjury))]
    [HarmonyPatch("ReduceDamageToPreserveOutsideParts")]
    public static class Transpiler_DamageWorker_AddInjury
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //StreamWriter writer = new StreamWriter(new FileStream("C:\\Users\\Vincent Wong\\Desktop\\output.txt", FileMode.Create));
            // Patch things up at the 2nd occurence of callvirt
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                {
                    occurencesCallvirt++;

                    if (occurencesCallvirt == 2)
                    {
                        /*
                        writer.WriteLine("Patching!");
                        writer.WriteLine(new CodeInstruction(OpCodes.Ldarga_S, 2));
                        writer.WriteLine(new CodeInstruction(OpCodes.Call, typeof(DamageInfo).GetProperty("HitPart").GetGetMethod()));
                        writer.WriteLine(new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth")));
                        */

                        yield return new CodeInstruction(OpCodes.Ldarga_S, 2);
                        yield return new CodeInstruction(OpCodes.Call, typeof(DamageInfo).GetProperty("HitPart").GetGetMethod());
                        yield return new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth"));

                        suppressCount = 1;
                        patchComplete = true;
                    }
                }

                if (suppressCount > 0)
                {
                    instruction.opcode = OpCodes.Nop;
                }

                //writer.WriteLine(instruction);
                yield return instruction;
            }

            //writer.Close();
        }

        private static void testMethod(float postArmorDamage, DamageInfo dinfo, Pawn pawn)
        {
            float maxHealth = dinfo.HitPart.def.GetMaxHealth(pawn, dinfo.HitPart);
        }
    }
}
