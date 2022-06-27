using EBF.Hediffs;
using EBF.Util;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Patches.Unification
{
    [HarmonyPatch(typeof(HediffComp))]
    [HarmonyPatch("CompTipStringExtra", MethodType.Getter)]
    public class PostFix_QualityBionics_CompPatches
    {
        public static bool Prepare()
        {
            return ModDetector.QualityBionicsIsLoaded;
        }

        /*
        public static MethodBase TargetMethod()
        {
            return AccessTools.PropertyGetter("QualityBionics.HediffCompQualityBionics:CompTipStringExtra");
        }
        */

        [HarmonyPostfix]
        public static void PostFix(HediffComp __instance, ref string __result)
        {
            HediffComp hediffComp = __instance as HediffComp;
            HediffCompProperties_MaxHPAdjust_Fake fakeProps = CommunityUnificationUtil.TryConvertQualityBionicsCompToFakeHpComp(hediffComp);
            if (fakeProps != null && __result == null)
            {
                // EliteBionicsFrameworkMain.LogError("fakeProps (postFix) " + fakeProps.ToStringSafe());
                __result = CommunityUnificationUtil.GetCompTipStringExtraDueToMaxHpAdjust(hediffComp.Pawn, hediffComp.parent.Part.def, fakeProps);
                // EliteBionicsFrameworkMain.LogError(__result);
            }
            // EliteBionicsFrameworkMain.LogError(__instance.ToStringSafe() + " " + __result.ToStringSafe());
            return;
        }
    }
}
