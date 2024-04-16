using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EBF.Transpilations.Moody
{
    [HarmonyPatch]
    public static class Transpiler_HealthCardUtility_GetTooltip
    {
        public static bool Prepare()
        {
            return ModDetector.MoodyIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
	        return AccessTools.Method("Moody.Extensions.RimWorld.HealthCardUtility:GetTooltip", new[] { typeof(IEnumerable<Hediff>), typeof(Pawn) });
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
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Hediff).GetProperty("Part").GetGetMethod()),
                    new CodeInstruction(OpCodes.Call, VanillaExtender.ReflectionGetMaxHealth())
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                .Set(OpCodes.Nop, null) // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
