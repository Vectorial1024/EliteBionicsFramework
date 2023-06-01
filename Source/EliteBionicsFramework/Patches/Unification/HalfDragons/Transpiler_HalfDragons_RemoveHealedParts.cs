using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Patches.Unification.HalfDragons
{
    [HarmonyPatch]
    public class Transpiler_HalfDragons_RemoveHealedParts
    {
        public static bool Prepare()
        {
            return ModDetector.HalfDragonsIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("HalfDragons.Need_DragonBlood:RemoveHealedParts");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * A total of 1 GetMaxHealth occurences detected;
             * Patch with CodeMatcher
             */
            return new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth)))
                ) // find the only occurence of .GetMaxHealth()
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, typeof(VanillaExtender).GetMethod("GetMaxHealth"))
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                .Set(OpCodes.Nop, null)
                // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
