using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations.Callouts
{
    [HarmonyPatch]
    public static class Transpiler_DamageResult_ThrowMotes
    {
        public static bool Prepare()
        {
            // compatibility with Callouts now handled by Mlie's variant
            // we keep this code around for future reference
            return false;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("CM_Callouts.DamageWorker_DamageResult_Patches+DamageWorker_DamageResult_AssociateWithLog:ThrowDestroyedPartMotes");
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
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldloc_S, 4),
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<BodyPartRecord>).GetProperty("Item").GetGetMethod()),
                    new CodeInstruction(OpCodes.Call, VanillaExtender.ReflectionGetMaxHealth())
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                .Set(OpCodes.Nop, null) // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
