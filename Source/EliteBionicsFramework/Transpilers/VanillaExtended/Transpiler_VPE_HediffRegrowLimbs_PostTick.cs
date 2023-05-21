using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Verse;

namespace EBF.Transpilers.VanillaExtended
{
    [HarmonyPatch]
    public class Transpiler_VPE_HediffRegrowLimbs_PostTick
    {
        public static bool Prepare()
        {
            return ModDetector.VanillaPsycastsExpandedIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("VanillaPsycastsExpanded.Hediff_RegrowLimbs:PostTick");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * This method intends to regrow limbs and give the regrown part a "regenerating" hediff.
             * The "regenerating" hediff one-off reads the max HP of the body part def.
             * Thus, we only need to replace the (one and only) GetMaxHealth with our EBF-provided GetMaxHealthUnmodified.
             * (Disclaimer: I do not have Royalty DLC, and I do not intend to get it in any near future.)
             * ---
             * This is also a very good opportunity to demonstrate how CodeMatcher works in practice.
             * Special shoutout to GitHub user delmain for introducing the idea of CodeMatcher!
             */
            return new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth)))
                ) // find the only occurence of .GetMaxHealth()
                .Set(OpCodes.Call, AccessTools.Method(typeof(EBFEndpoints), nameof(EBFEndpoints.GetMaxHealthUnmodified)))
                // and replace with our safe GetMaxHealthUnmodified()
                .InstructionEnumeration();
        }
    }
}
