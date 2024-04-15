using EBF.Hediffs;
using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(VerbProperties))]
    [HarmonyPatch(nameof(VerbProperties.AdjustedMeleeDamageAmount), MethodType.Normal)]
    [HarmonyPatch(new Type[] { typeof(Tool), typeof(Pawn), typeof(Thing), typeof(HediffComp_VerbGiver) })]
    public class PostFix_VerbProperties_AdjustedMeleeDamageAmount
    {
        public static void PostFix(ref float __result, Tool tool, Pawn attacker, HediffComp_VerbGiver hediffCompSource)
        {
            // __result already has the scaling multipliers within it.
            if (attacker != null)
            {
                // There is an attacker.
                int tempLinearAdjustment = 0;
                float tempScalingAdjustment = 1;
                // Handle several cases.
                if (ToolFinderUtils.ToolIsOriginalToolOfPawn(tool, attacker))
                {
                    // Attacks by bare-hand; no hediff source.
                    // May have implant or no implant...
                    BodyPartGroupDef hostGroup = tool.linkedBodyPartsGroup;
                    List<HediffWithComps> hediffList = new List<HediffWithComps>();
                    attacker.health.hediffSet.GetHediffs(ref hediffList);
                    foreach (HediffWithComps candidateHediff in hediffList)
                    {
                        if (!(candidateHediff is Hediff_Implant))
                        {
                            // Normal hediff only.
                            continue;
                        }
                        if (candidateHediff.Part.IsInGroup(hostGroup))
                        {
                            // Relevant.
                            HediffComp_ToolPowerAdjust adjustmentComps = candidateHediff.TryGetComp<HediffComp_ToolPowerAdjust>();
                            if (adjustmentComps != null)
                            {
                                // Have adjustment comps
                                __result += adjustmentComps.Props.linearAdjustment;
                            }
                        }
                    }
                }
                else if (hediffCompSource != null)
                {
                    // Attacks by bionic parts; bionic may have power adjustment components.
                    if (ToolPowerAdjuster.CalculatePowerAdjustmentDueToImplants(attacker, hediffCompSource.parent.Part, ref tempLinearAdjustment, ref tempScalingAdjustment))
                    {
                        // Successfully calculated.
                        __result += tempLinearAdjustment;
                    }
                    if (ToolPowerAdjuster.CalculatePowerAdjustmentDueToToolUpgrade(attacker, hediffCompSource.parent.Part, tool, ref tempLinearAdjustment, ref tempScalingAdjustment))
                    {
                        // Successfully calculated.
                        __result += tempLinearAdjustment;
                    }
                }
            }
        }
    }
}
