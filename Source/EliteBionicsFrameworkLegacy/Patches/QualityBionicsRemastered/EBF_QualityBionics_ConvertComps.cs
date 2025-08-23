using EBF.Hediffs;
using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace EliteBionicsFrameworkLegacy.Patches.QualityBionicsRemastered
{
    [HarmonyPatch]
    internal class EBF_QualityBionics_ConvertComps
    {
        internal static bool IsCorrectRimWorldVersion()
        {
            // only do this for RimWorld 1.5
            var version = RimWorldDetector.RimWorldVersion;
            return version >= new Version(1, 5) && version < new Version(1, 6);
        }

        internal static bool HasCorrectLoadedMod()
        {
            return ModDetector.QualityBionicsRemasteredIsLoaded;
        }

        public static bool Prepare()
        {
            return IsCorrectRimWorldVersion() && HasCorrectLoadedMod();
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("EBF.Util.CommunityUnificationUtil:TryConvertQualityBionicsContinuedCompToFakeHpComp");
        }

        [HarmonyPrefix]
        public static bool ReplaceContinuedVariantUnification(ref HediffCompProperties_MaxHPAdjust_Fake __result, HediffComp comp)
        {
            // we detect the Remastered variant so that we no longer need to deal with the continued variant
            if (EBF_QualityBionics_CollectTypes.QualityBionicsRemastered_Type_CompQualityBionics.IsInstanceOfType(comp))
            {
                // is instance of comp
                QualityCategory quality = (QualityCategory)EBF_QualityBionics_CollectTypes.QualityBionicsRemastered_Type_CompQualityBionics.GetField("quality").GetValue(comp);
                // EliteBionicsFrameworkMain.LogError("quality " + quality.ToStringSafe());
                float scalingMultiplier = EBF_QualityBionics_Reverse_GetQualityMultipliers.GetQualityMultipliersForHP(quality);
                // float scalingMultiplier = (float) QualityBionics_Method_GetQualityMultiplier.Invoke(qualityBionicsSettings, new object[] { quality });
                // EliteBionicsFrameworkMain.LogError("scaler " + scalingMultiplier.ToStringSafe());
                HediffCompProperties_MaxHPAdjust_Fake fakeComp = new HediffCompProperties_MaxHPAdjust_Fake
                {
                    linearAdjustment = 0,
                    scaleAdjustment = scalingMultiplier - 1,
                    providerNamespace = EBF_QualityBionics_CollectTypes.QualityBionicsRemastered_Type_CompQualityBionics.Namespace
                };
                // EliteBionicsFrameworkMain.LogError("fakeComp " + fakeComp.ToStringSafe());
                __result = fakeComp;
            }

            return false;
        }
    }
}
