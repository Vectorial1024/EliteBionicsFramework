using EBF.Util;
using HarmonyLib;
using System;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(VerbProperties))]
    [HarmonyPatch(nameof(VerbProperties.AdjustedMeleeDamageAmount), MethodType.Normal)]
    [HarmonyPatch(new Type[] { typeof(Tool), typeof(Pawn), typeof(Thing), typeof(HediffComp_VerbGiver) })]
    public class Mixed_VerbProperties_AdjustedMeleeDamageAmount
    {
        /// <summary>
        /// Prepares the tool power adjustment info, in case it is needed later on.
        /// </summary>
        [HarmonyPrefix]
        public static void PrepareAdjustmentInformation(ref ToolPowerAdjustInfo? __state, Tool tool, Pawn attacker, HediffComp_VerbGiver hediffCompSource)
        {
            ToolPowerAdjustInfo? powerAdjustInfo = EBFEndpoints.GetToolPowerAdjustInfoWithEbf(attacker, tool, hediffCompSource);
            __state = powerAdjustInfo;
        }

        /// <summary>
        /// Recalculates the actual tool power if needed.
        /// </summary>
        /// <param name="__result"></param>
        [HarmonyPostfix]
        public static void RecalculateBaseMeleeDamageAmount(VerbProperties __instance, ref float __result, ref ToolPowerAdjustInfo? __state, Tool tool, Pawn attacker, Thing equipment, HediffComp_VerbGiver hediffCompSource)
        {
            if (!__state.HasValue)
            {
                // invalid; nothing to do
                // this checks that e.g. pawn (attacker) is not null
                return;
            }
            ToolPowerAdjustInfo adjustInfo = __state.Value;
            if (!adjustInfo.HasAdjustment)
            {
                // no adjustment; nothing to do
                return;
            }
            // has adjustment; what kind of adjustment?
            if (hediffCompSource == null)
            {
                // not from a hediff, so definitely not from a tool upgrade.
                // just apply the changes and then can finish
                __result *= adjustInfo.scalingAdj;
                __result += adjustInfo.linearAdj;
                return;
            }
            // is from hediff; we need to check for tool upgrades.
            Tool originalTool = ToolFinderUtils.FindCorrespondingOriginalToolInBaseBody(tool, attacker, hediffCompSource.parent.Part);
            if (originalTool == null)
            {
                // no valid upgrades; nothing to do
                return;
            }
            // has valid upgrade; essentially, recalculate the thing.
            float newVal = OriginalAdjustedMeleeDamageAmount(originalTool, attacker, equipment, null);
            newVal *= adjustInfo.scalingAdj;
            newVal += adjustInfo.linearAdj;
            __result = newVal;
        }

        [HarmonyReversePatch]
        public static float OriginalAdjustedMeleeDamageAmount(Tool tool, Pawn attacker, Thing equipment, HediffComp_VerbGiver hediffCOmpSource)
        {
            // this gets the original AdjustedMeleeDamageAmount in case we want a recalculation
            throw new NotImplementedException("Called a stub before reverse patching is complete.");
        }
    }
}
