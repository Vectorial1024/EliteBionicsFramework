using EBF.Hediffs;
using EBF.Util;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace EBF.Patches.Unification
{
    /// <summary>
    /// Situation: HediffComp.CompTipStringExtra is virtual, which means we cannot specifically target individual types to specifically modify their tip labels.
    /// <para/>
    /// However, this also means we can make a general patch that covers everything.
    /// <para/>
    /// We will use brute force to check the type of each HediffComp, and apply the patches if the type matches known types.
    /// </summary>
    [HarmonyPatch(typeof(HealthCardUtility))]
    [HarmonyPatch("GetTooltip")]
    public class PostFix_Everyone_BodyPartTooltip
    {
        [HarmonyPostfix]
        public static void PostFix(Pawn pawn, BodyPartRecord part, ref string __result)
        {
            string temp = CommunityUnificationUtil.GetBodyPartSummaryTooltipStringDueToMaxHpAdjust(pawn, part);
            if (temp != null)
            {
                StringBuilder builder = new StringBuilder(__result);
                builder.AppendLine(temp);
                __result = builder.ToString();
            }
        }
    }
}
