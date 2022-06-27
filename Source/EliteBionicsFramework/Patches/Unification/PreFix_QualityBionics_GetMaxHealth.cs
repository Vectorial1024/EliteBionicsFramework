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
    [HarmonyPatch]
    public class PreFix_QualityBionics_GetMaxHealth
    {
        public static bool Prepare()
        {
            return ModDetector.QualityBionicsIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("QualityBionics.GetMaxHealth_Patch:Postfix");
        }

        [HarmonyPrefix]
        public static bool DoNotDoThePatch()
        {
            // we extend the generosity of fixing Quality Bionic's wrong implementation by unifying them under our management
            return false;
        }
    }
}
