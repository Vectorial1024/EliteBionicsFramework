using EBF.Util;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EBF.Patches.Unification
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
