using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch]
    public class PostFix_Pawnmorpher_HealthUtil
    {
        public static bool Prepare()
        {
            return ModDetector.PawnmorpherIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.BodyUtilities:GetPartMaxHealth");
        }

        [HarmonyPrefix]
        public static void PreFix()
        {
            // the flow has changed
            // we approve of Pawnmorpher reading the original values, and then we modify their value to become an EBF-accepted value
            Prefix_BodyPart_GetMaxHealth.SuppressNextWarning();
        }
    }
}
