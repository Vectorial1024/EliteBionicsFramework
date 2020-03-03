using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;
using static Verse.DamageWorker;

namespace EBF.Transpilations
{
    [HarmonyPatch(typeof(DamageWorker_Blunt))]
    [HarmonyPatch("ApplySpecialEffectsToPart")]
    public static class Transpiler_DamageWorker_Blunt_SpecialEffects
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Patch things up at the 11th occurence of callvirt
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
                        Type targetType = null;
                        // There is a compiler-generated class, and it is named as <>c__DisplayClass1_0
                        // Perhaps previously it was named something like ApplySpecialEffectsToPart, IDK
                        foreach (var info in typeof(DamageWorker_Blunt).GetNestedTypes(BindingFlags.NonPublic))
                        {   
                            if (info.Name.Contains("DisplayClass1_0"))
                            {
                                targetType = info;
                            }
                        }

                        List<CodeInstruction> insert = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldloc_0),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(targetType, "pawn")),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Thing), "def")),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingDef), "race")),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RaceProperties), "body")),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BodyDef), "corePart")),
                            new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth"))
                        };
                        foreach (CodeInstruction command in insert)
                        {
                            yield return command;
                        }

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
