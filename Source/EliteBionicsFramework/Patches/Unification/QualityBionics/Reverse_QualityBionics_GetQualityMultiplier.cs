using EBF.Util;
using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;

namespace EBF.Patches.Unification.QualityBionics
{
    [HarmonyPatch]
    public class Reverse_QualityBionics_GetQualityMultiplier
    {
        public static bool Prepare()
        {
            return ModDetector.QualityBionicsIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("QualityBionics.QualityBionicsSettings:GetQualityMultipliersForHP");
        }

        [HarmonyReversePatch]
        public static float GetQualityMultipliersForHP(object __instance, QualityCategory quality)
        {
            throw new NotImplementedException("Called a stub before reverse patching is complete.");
        }
    }
}
