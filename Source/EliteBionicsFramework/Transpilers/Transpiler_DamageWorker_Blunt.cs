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
    // disabled due to unexplainable ("magical") incompatibility with Humanoid Alien Races
    //[HarmonyPriority(Priority.First)]
    //[HarmonyPatch(typeof(DamageWorker_Blunt))]
    //[HarmonyPatch("ApplySpecialEffectsToPart")]
    public static class Transpiler_DamageWorker_Blunt_SpecialEffects
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Patch things up at the 11th occurence of callvirt
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;

            /*
             * This is to patch for "get core part max health" in the function ApplySpecialEffectsToPart(...)
             * for class DamageWorker_Blunt.
             * From what we know, RW generates a lot of nested compiler-generated class,
             * and among them, there is one under DamageWorker_Blunt that stores info related to the blunt-stun effects.
             * 
             * This generated class may have different names, but from what we know, this class:
             * - has a field named "pawn"
             * - such field has type Pawn (this should be less important, since we can't have two fields with the same name but with different types)
             * 
             * I am assuming there is only one such class among all the nested classes under DamageWorker_Blunt.
             */
            Type typeSelfAnon = typeof(DamageWorker_Blunt).GetNestedTypes(BindingFlags.NonPublic).Where((Type type) => type.GetField("pawn") != null).First();
            if (typeSelfAnon != null)
            {
                // Search successful.
                foreach (CodeInstruction instruction in instructions)
                {
                    if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                    {
                        occurencesCallvirt++;

                        if (occurencesCallvirt == 11)
                        {

                            List<CodeInstruction> insert = new List<CodeInstruction>()
                            {
                                new CodeInstruction(OpCodes.Ldloc_0),
                                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeSelfAnon, "pawn")),
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
                yield break;
            }
            else
            {
                // In the unlikely case of failing the search, modify nothing and return.
                foreach (CodeInstruction instruction in instructions)
                {
                    yield return instruction;
                }
                yield break;
            }
        }
    }
}
