using EBF.Util;
using HarmonyLib;
using System.Reflection;

namespace EBF.Patches.Unification.QualityBionicsContinued
{
    [HarmonyPatch]
    public class PreFix_QualityBionicsContinued_GetMaxHealth
    {
        public static bool Prepare()
        {
            return ModDetector.QualityBionicsContinuedIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("QualityBionicsContinued.Patch.BodyPartDef_GetMaxHealth:Postfix");
        }

        [HarmonyPrefix]
        public static bool DoNotDoThePatch()
        {
            // we extend the generosity of fixing Quality Bionic's wrong implementation by unifying them under our management
            return false;
        }
    }
}
