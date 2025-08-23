using HarmonyLib;
using System;
using System.Reflection;

namespace EliteBionicsFrameworkLegacy.Patches.QualityBionicsRemastered
{
    [HarmonyPatch]
    internal class EBF_QualityBionics_Suppress_Reverse_GetQualityMultipliers
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
            return AccessTools.Method("EBF.Patches.Unification.QualityBionicsContinued.Reverse_QualityBionicsContinued_GetQualityMultiplier:Prepare");
        }

        [HarmonyPrefix]
        public static bool SuppressPatch()
        {
            return false;
        }
    }
}
