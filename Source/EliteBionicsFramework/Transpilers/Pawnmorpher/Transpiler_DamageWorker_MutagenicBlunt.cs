﻿using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations.Pawnmorpher
{
    [StaticConstructorOnStartup]
    public static class Transpiler_DamageWorker_MutagenicBlunt_SpecialEffects
    {
        /* 
         * Fix transpiler conflict with Humanoid Alien Races
         * HAR's HarmonyPatches.BodyReferenceTranspiler method patches DamageWorker_Blunt.ApplySpecialeffectsToPart method also.
         * Fortunately both transpilers will work if EBF's transpiler is applied first, but this requires that EBF is loaded
         * before HAR, and that EBF's harmony patch is done in the static constructor because HAR's harmony patch is done
         * in a static constructor.
         * 
         * Special thanks to GitHub user RocketDelivery for this fix
         * 
         * ------
         * 
         * Update 2022-Nov:
         * 
         * The fix by RocketDelivery works when used with HAR, but we eventually discovered Yayo Animations doing their patches super-early,
         * and therefore summonned Humanoid Alien Races too early without our consent. This therefore breaks our compatiility with HAR.
         * To maintain parity against Yayo Animations, we must therefore call this static constructor in the constructor of a child class of Verse.Mod.
         * I have made a new fitting child class that, innu, touches this class, so that this static constructor is called.
         * And obviously, the [StaticConstructorOnStartup] becomes a decoration, but I am keeping that annotation to avoid confusion.
         */
        static Transpiler_DamageWorker_MutagenicBlunt_SpecialEffects()
		{
            Harmony harmony = new Harmony("rimworld.vectorial1024.ebf.damageworker_mutagenicblunt");
            if (!ModDetector.PawnmorpherIsLoaded)
            {
                // nope
                return;
            }
            harmony.Patch(
                AccessTools.Method(AccessTools.TypeByName("Pawnmorph.Damage.Worker_MutagenicBlunt"), "ApplySpecialEffectsToPart"), 
                null,
                null,
                new HarmonyMethod(typeof(Transpiler_DamageWorker_MutagenicBlunt_SpecialEffects), nameof(Transpiler)));
		}

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Patch things up at the 11th occurence of callvirt
            short occurencesCallvirt = 0;
            short suppressCount = 0;
            bool patchComplete = false;
            // Harmony.DEBUG = true;

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
            Type typeSelfAnon = AccessTools.TypeByName("Pawnmorph.Damage.Worker_MutagenicBlunt").GetNestedTypes(BindingFlags.NonPublic).Where((Type type) => type.GetField("pawn") != null).First();
            if (typeSelfAnon != null)
            {
                // FileLog.Log("Lettuce begin");
                // Search successful.
                foreach (CodeInstruction instruction in instructions)
                {
                    if (!patchComplete && instruction.opcode == OpCodes.Callvirt)
                    {
                        occurencesCallvirt++;

                        if (occurencesCallvirt == 9)
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
                                FileLog.Log(command.ToString());
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

                    FileLog.Log(instruction.ToString());
                    yield return instruction;
                }
            }
            else
            {
                EliteBionicsFrameworkMain.LogError("Patch failed: Pawnmorpher mutagenic blunt special effects, failed to find relevant self-anon type!");
                // In the unlikely case of failing the search, modify nothing and return.
                foreach (CodeInstruction instruction in instructions)
                {
                    yield return instruction;
                }
            }
            // Harmony.DEBUG = false;
            yield break;
        }
    }
}
