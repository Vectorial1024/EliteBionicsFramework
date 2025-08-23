using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;

namespace EliteBionicsFrameworkLegacy.Patches.QualityBionicsRemastered
{
    [HarmonyPatch]
    internal class EBF_QualityBionics_Reverse_GetQualityMultipliers
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
            return AccessTools.Method("QualityBionicsRemastered.Settings:GetQualityMultipliersForHP");
        }

        [HarmonyReversePatch]
        public static float GetQualityMultipliersForHP(QualityCategory quality)
        {
            throw new NotImplementedException("Called a stub before reverse patching is complete.");
        }
    }
}
