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
                // EliteBionicsFrameworkMain.LogError("Power (no source): " + __result + " adjust info " + adjustInfo.ToString());
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
            // since I can't seem to tell it to recalculate with the original tool as the parameter, we will have to physically look at the source code.
            // we discover the original source code only does multiplication, so we can infer what should be the damage buff that was applied
            float replacementBasePower = tool.power;
            float powerScale = (__result / replacementBasePower);
            // EliteBionicsFrameworkMain.LogError("Game calculates value as " + __result + "; therefore the ratio is " + powerScale);
            float newVal = originalTool.power * powerScale;
            // float newVal = OriginalAdjustedMeleeDamageAmount(__instance, originalTool, attacker, equipment, null);
            newVal *= adjustInfo.scalingAdj;
            newVal += adjustInfo.linearAdj;
            __result = newVal;
            // EliteBionicsFrameworkMain.LogError("Power (hediff upgrade): " + __result + " adjust info " + adjustInfo.ToString() + " newVal " + newVal);
        }
    }
}
