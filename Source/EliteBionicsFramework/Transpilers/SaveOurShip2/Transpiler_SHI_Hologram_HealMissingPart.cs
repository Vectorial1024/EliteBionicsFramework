using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Verse;

namespace EBF.Transpilers.VanillaExtended
{
    [HarmonyPatch]
    public class Transpiler_SHI_Hologram_HealMissingPart
    {
        public static bool Prepare()
        {
            return ModDetector.SaveOurShip2IsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("ShipsHaveInsides.HediffPawnIsHologram:HealMissingPart");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * This method intends to regrow limbs and give the regrown part a generic bruise hediff.
             * The generic bruise hediff one-off reads the max HP of the body part def.
             * Thus, we only need to replace the (one and only) GetMaxHealth with our EBF-provided GetMaxHealthUnmodified.
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
