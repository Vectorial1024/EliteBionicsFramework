using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace EBF.Patches.AlteredCarbon
{
    [HarmonyPatch]
    public class PreFix_AC2_SleeveGestator_Repurposing_Lambda1
    {
        public static bool Prepare()
        {
            return ModDetector.AlteredCarbon2IsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            // we need to find the 2 lambdas inlined by the compiler
            // first find the type...
            Type theType = AccessTools.TypeByName("AlteredCarbon.Building_SleeveGestator");
            // then find the methods with the signature func(Hediff_MissingPart)
            IEnumerable<MethodInfo> candidateMethods = theType.GetMethods(BindingFlags.NonPublic).Where((MethodInfo method) => {
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].GetType() == typeof(Hediff_MissingPart);
            });
            // since Harmony can only target 1 method at a time, pick only one of them
            return candidateMethods.ElementAt(0);
        }

        [HarmonyPrefix]
        public static void AllowGetMaxHealth(Pawn pawn, BodyPartRecord part, ref string __result)
        {
            // since it involves missing parts, there cannot be any other hediffs attached to it, so calling GetMaxHealth is permitted.
            // just suppress the warning, and then is ok
            PostFix_BodyPart_GetMaxHealth.SuppressNextWarning();
            return;
        }
    }
}
