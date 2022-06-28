using EBF.Hediffs;
using EBF.Util;
using HarmonyLib;
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
    [HarmonyPatch("CompTipStringExtra", MethodType.Getter)]
    public class PostFix_Everyone_CompPatches_TipString
    {
        [HarmonyPostfix]
        public static void PostFix(HediffComp __instance, ref string __result)
        {
            HediffCompProperties_MaxHPAdjust_Fake fakeCompsQualityBionics = CommunityUnificationUtil.TryConvertQualityBionicsCompToFakeHpComp(__instance);
            if (fakeCompsQualityBionics != null)
            {
                // they are supposed to only have null __result, so we can just set it and finish
                __result = CommunityUnificationUtil.GetCompTipStringExtraDueToMaxHpAdjust(__instance.Pawn, __instance.parent.Part.def, fakeCompsQualityBionics);
                return;
            }
            HediffCompProperties_MaxHPAdjust_Fake fakeCompsCONN = CommunityUnificationUtil.TryConvertConnCompToFakeHpComp(__instance);
            if (fakeCompsCONN != null)
            {
                // they have their custom "Health points added" string, and we will still override it
                __result = CommunityUnificationUtil.GetCompTipStringExtraDueToMaxHpAdjust(__instance.Pawn, __instance.parent.Part.def, fakeCompsCONN);
                return;
            }
            // check other types as needed
        }
    }
}
