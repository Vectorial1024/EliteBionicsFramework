using EBF;
using EBF.Hediffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Util
{
    public class ToolPowerAdjuster
    {
        public static bool CalculatePowerAdjustmentDueToImplants(Pawn pawn, BodyPartRecord part, ref int linearAdjustment, ref float scalingAdjustment)
        {
            if (pawn.health == null)
            {
                // Impossible. The pawn is bugged anyways.
                linearAdjustment = 0;
                scalingAdjustment = 1;
                return false;
            }

            int linearAdj = 0;
            float scalingAdj = 1;
            bool foundComps = false;
            // Find all implant-hediffs inside the interested BodyPartRecord
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.Part == part && hediff is Hediff_Implant)
                {
                    // Interested. Determine if it carries with it any VerbPowerAdjust components
                    HediffCompProperties_ToolPowerAdjust adjustorComp = hediff.def.CompProps<HediffCompProperties_ToolPowerAdjust>();
                    if (adjustorComp != null)
                    {
                        linearAdj += adjustorComp.linearAdjustment;
                        scalingAdj *= adjustorComp.ActualScalingFactor;
                        foundComps = true;
                    }
                }
            }

            linearAdjustment = linearAdj;
            scalingAdjustment = scalingAdj;
            return foundComps;
        }

        public static bool CalculatePowerAdjustmentDueToToolUpgrade(Pawn attacker, BodyPartRecord attackingBodyPart, Tool attackingTool, ref int linearAdjustment, ref float scalingAdjustment)
        {
            if (attacker.health == null)
            {
                // Impossible. The pawn is bugged anyways.
                linearAdjustment = 0;
                scalingAdjustment = 1;
                return false;
            }

            Tool originalTool = ToolFinderUtils.FindCorrespondingOriginalToolInBaseBody(attackingTool, attacker, attackingBodyPart);
            if (originalTool != null)
            {
                // Valid upgrade. Apply adjustments.
                int linearAdj = 0;
                float scalingAdj = 1;
                // Find all non-implant hediffs inside the interested BodyPartRecord
                foreach (Hediff hediff in attacker.health.hediffSet.hediffs)
                {
                    if (hediff.Part == attackingBodyPart && !(hediff is Hediff_Implant))
                    {
                        // Interested. Determine if it carries with it any VerbPowerAdjust components
                        HediffCompProperties_ToolPowerAdjust adjustorComp = hediff.def.CompProps<HediffCompProperties_ToolPowerAdjust>();
                        if (adjustorComp != null)
                        {
                            linearAdj += adjustorComp.linearAdjustment;
                            scalingAdj *= adjustorComp.ActualScalingFactor;
                        }
                    }
                }

                linearAdjustment = linearAdj;
                scalingAdjustment = scalingAdj;
                return true;
            }
            else
            {
                // Not an upgrade. Apply no adjustments.
                linearAdjustment = 0;
                scalingAdjustment = 1;
                return false;
            }
        }
    }
}
