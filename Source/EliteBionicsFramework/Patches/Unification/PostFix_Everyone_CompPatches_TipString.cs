using EBF.API;
using EBF.Extensions;
using EBF.Hediffs;
using EBF.Util;
using HarmonyLib;
using System.Collections.Generic;
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
    [HarmonyPatch(typeof(HediffComp))]
    [HarmonyPatch(nameof(HediffComp.CompTipStringExtra), MethodType.Getter)]
    public class PostFix_Everyone_CompPatches_TipString
    {
        [HarmonyPostfix]
        public static void PostFix(HediffComp __instance, ref string __result)
        {
            HediffCompProperties_MaxHPAdjust_Fake fakeCompsQualityBionicsContinued = CommunityUnificationUtil.TryConvertQualityBionicsContinuedCompToFakeHpComp(__instance);
            if (fakeCompsQualityBionicsContinued != null)
            {
                __result = CommunityUnificationUtil.GetCompTipStringExtraDueToMaxHpAdjust(__instance.Pawn, __instance.parent.Part.def, fakeCompsQualityBionicsContinued);
                return;
            }
            // notes: due to CONN officially changing to use EBF directly, we no longer need to check for CONN comps
            // check other types as needed

            if (__instance.TryExtractEbfExternalCompProps(out var ebfExternalComp))
            {
                __result = CommunityUnificationUtil.GetCompTipStringExtraDueToMaxHpAdjust(__instance.Pawn, __instance.parent.Part.def, ebfExternalComp);
            }
        }
    }
}
