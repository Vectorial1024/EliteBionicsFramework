using EBF.Hediffs;
using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace EBF.Patches.Unification
{
    [HarmonyPatch(typeof(HediffWithComps))]
    [HarmonyPatch("LabelInBrackets", MethodType.Getter)]
    public class PostFix_Everyone_HediffWithComps_LabelBracket
    {
        [HarmonyPostfix]
        public static void PostFix(HediffWithComps __instance, ref string __result)
        {
            StringBuilder builder = new StringBuilder(__result);
            List<HediffCompProperties_MaxHPAdjust> realAndFakeProps = CommunityUnificationUtil.GetRealAndFakeHpPropsForUnification(__instance);
            if (realAndFakeProps.Count > 0)
            {
                if (builder.Length > 0)
                {
                    builder.Append("; ");
                }
                builder.Append(CommunityUnificationUtil.GetCompLabelInBracketsDueToMaxHpAdjust(__instance.pawn, __instance));
                __result = builder.ToString();
            }
        }
    }
}
