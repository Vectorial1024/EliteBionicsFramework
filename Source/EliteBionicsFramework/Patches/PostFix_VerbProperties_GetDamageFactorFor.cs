using EBF;
using EliteBionicsFramework.Hediffs;
using EliteBionicsFramework.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EliteBionicsFramework.Patches
{
    [HarmonyPatch(typeof(VerbProperties))]
    [HarmonyPatch("GetDamageFactorFor", MethodType.Normal)]
    [HarmonyPatch(new Type[] { typeof(Tool), typeof(Pawn), typeof(HediffComp_VerbGiver) })]
    public class PostFix_VerbProperties_GetDamageFactorFor
    {
        [HarmonyPostfix]
        public static void PostFix(ref float __result, Tool tool, Pawn attacker, HediffComp_VerbGiver hediffCompSource)
        {
            //EliteBionicsFrameworkMain.LogError("Dump vars: " + __result + ", " + tool.LabelCap + ", " + attacker.Name + ", " + hediffCompSource);
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
                    foreach (HediffWithComps candidateHediff in attacker.health.hediffSet.GetHediffs<HediffWithComps>())
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
                                __result *= adjustmentComps.Props.ActualScalingFactor;
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
                        __result *= tempScalingAdjustment;
                    }
                    if (ToolPowerAdjuster.CalculatePowerAdjustmentDueToToolUpgrade(attacker, hediffCompSource.parent.Part, tool, ref tempLinearAdjustment, ref tempScalingAdjustment))
                    {
                        // Successfully calculated.
                        __result *= tempScalingAdjustment;
                    }
                }
            }
        }
    }
}
