using EBF.Util;
using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;

namespace EBF.Patches.Unification.QualityBionicsContinued
{
    [HarmonyPatch]
    public class Reverse_QualityBionicsContinued_GetQualityMultiplier
    {
        public static bool Prepare()
        {
            // patch deprecated, but we retain code for future reference
            return false;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("QualityBionicsContinued.Settings:GetQualityMultipliersForHP");
        }

        [HarmonyReversePatch]
        public static float GetQualityMultipliersForHP(QualityCategory quality)
        {
            throw new NotImplementedException("Called a stub before reverse patching is complete.");
        }
    }
}
