using EBF.Hediffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Util
{
    /// <summary>
    /// A struct for storing the verb attack info of a (melee) attack, for convenient identification and calculation.
    /// </summary>
    /// <param name="tool"></param>
    /// <param name="source"></param>
    public readonly struct VerbAttackInfo(Tool tool, HediffComp_VerbGiver source) : IEquatable<VerbAttackInfo>
    {
        public readonly Tool tool = tool;
        public readonly HediffComp_VerbGiver source = source;

        public bool IsValid
        {
            get
            {
                // the tool must exist, but the verbgiver source can be optional (when given by e.g. genes, special hediffs, etc. etc.)
                return tool != null;
            }
        }

        public bool Equals(VerbAttackInfo other)
        {
            return tool == other.tool && source == other.source;
        }

        public List<HediffComp_ToolPowerAdjust> ExtractRelevantHediffCompsFromPawn(Pawn pawn)
        {
            if (!IsValid)
            {
                return [];
            }
            List<HediffComp_ToolPowerAdjust> theList = [];
            // what kind of attack info is this?
            if (ToolFinderUtils.ToolIsOriginalToolOfPawn(tool, pawn))
            {
                /*
                 * a natural tool (from body part tree) was used
                 * we will check for implants and extract the relevant comps
                 * 
                 * we need to check for natural tools to exclude equipment e.g. beer bottles; such items also count as tools, together with natural verbs
                 */
                return ExtractForNaturalTool(pawn);
            }
            else if (source != null)
            {
                /*
                 * a hediff tool (from bionic/implant) was used; there are several possible sources:
                 * - bionic providing an improved version of the old verb (e.g. bionic arm) that boosts verb power
                 * - implant boosts the effectiveness of some other bionics (e.g. (hypothetical) portable minified power core)
                 */
                return ExtractForHediffTool(pawn);
            }
            return theList;
        }

        private List<HediffComp_ToolPowerAdjust> ExtractForNaturalTool(Pawn pawn)
        {
            BodyPartGroupDef hostGroup = tool.linkedBodyPartsGroup;
            List<HediffWithComps> hediffList = [];
            List<HediffComp_ToolPowerAdjust> resultList = [];
            pawn.health.hediffSet.GetHediffs(ref hediffList);
            foreach (HediffWithComps candidateHediff in hediffList)
            {
                if (candidateHediff is not Hediff_Implant)
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
                        resultList.Add(adjustmentComps);
                    }
                }
            }
            return resultList;
        }

        private List<HediffComp_ToolPowerAdjust> ExtractForHediffTool(Pawn pawn)
        {
            if (pawn.health == null)
            {
                // impossible; the pawn is bugged
                return [];
            }

            List<HediffComp_ToolPowerAdjust> resultList = [];
            BodyPartRecord bodyPartSource = source.parent.Part;
            // find implants on the exact part first
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.Part == bodyPartSource && hediff is Hediff_Implant)
                {
                    // Interested. Determine if it carries with it any VerbPowerAdjust components
                    HediffComp_ToolPowerAdjust adjustmentComps = hediff.TryGetComp<HediffComp_ToolPowerAdjust>();
                    if (adjustmentComps != null)
                    {
                        // Have adjustment comps
                        resultList.Add(adjustmentComps);
                    }
                }
            }
            // then, find hediffs with verb replacement and load the comps
            Tool originalTool = ToolFinderUtils.FindCorrespondingOriginalToolInBaseBody(tool, pawn, bodyPartSource);
            if (originalTool != null)
            {
                // it is a replacemnet
                // we do not check the base value here; we simply aggregate the adjustment
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff.Part == bodyPartSource && hediff is not Hediff_Implant)
                    {
                        // Interested. Determine if it carries with it any VerbPowerAdjust components
                        HediffComp_ToolPowerAdjust adjustmentComps = hediff.TryGetComp<HediffComp_ToolPowerAdjust>();
                        if (adjustmentComps != null)
                        {
                            // Have adjustment comps
                            resultList.Add(adjustmentComps);
                        }
                    }
                }
            }

            // finish
            return resultList;
        }
    }
}
