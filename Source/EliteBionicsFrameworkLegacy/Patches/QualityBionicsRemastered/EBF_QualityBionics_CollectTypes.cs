using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace EliteBionicsFrameworkLegacy.Patches.QualityBionicsRemastered
{
    [HarmonyPatch]
    internal class EBF_QualityBionics_CollectTypes
    {
        internal static MethodInfo RW_Hediff_TryGetComp = null;

        internal static Type QualityBionicsRemastered_Type_Settings = null;
        internal static MethodInfo QualityBionicsRemastered_Method_GetQualityMultiplier = null;

        internal static Type QualityBionicsRemastered_Type_CompQualityBionics = null;
        internal static MethodInfo QualityBionicsRemastered_TryGetRelevantComp = null;

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
            return AccessTools.Method("EBF.Util.CommunityUnificationUtil:TryPatchQualityBionicsContinued");
        }

        [HarmonyPrefix]
        public static bool ReplaceContinuedVariantUnification()
        {
            // we detect the Remastered variant so that we no longer need to deal with the continued variant
            var methodSignature = new Type[] { typeof(Hediff) };
            RW_Hediff_TryGetComp = typeof(HediffUtility).GetMethod(nameof(HediffUtility.TryGetComp), methodSignature);
            methodSignature = null;

            try
            {
                QualityBionicsRemastered_Type_Settings = Type.GetType("QualityBionicsRemastered.Settings");
                QualityBionicsRemastered_Method_GetQualityMultiplier = QualityBionicsRemastered_Type_Settings.GetMethod("GetQualityMultipliersForHP");

                QualityBionicsRemastered_Type_CompQualityBionics = Type.GetType("QualityBionicsRemastered.HediffCompQualityBionics");
                var tempType = new Type[] { QualityBionicsRemastered_Type_CompQualityBionics };
                QualityBionicsRemastered_TryGetRelevantComp = RW_Hediff_TryGetComp.MakeGenericMethod(tempType);
                tempType = null;
            }
            catch (ArgumentNullException)
            {
                EBFLegacy.LogError("Something about Quality Bionics Remastered (for RimWorld 1.5) changed; please report this to us.");
            }

            // do not do the original code!
            return false;
        }
    }
}
