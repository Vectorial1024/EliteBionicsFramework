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
            return ModDetector.PawnmorpherIsLoaded && false;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.BodyUtilities:GetPartMaxHealth");
        }

        [HarmonyPostfix]
        public static void PostFix(ref float __result, BodyPartRecord record, Pawn p)
        {
            // we need to deduce what their OFFSET value is, and then use that OFFSET value to recalculate the correct value
            // this will cause a little bit of inaccuracy but oh well. it cant be avoided.
            // the original formula was: offset * pawn.healthScale + record.GetPartMaxHealth(p)
            float pawnmorpherOffsetValue = __result - record.def.GetRawMaxHealth(p);
            __result = pawnmorpherOffsetValue + record.GetMaxHealthForBodyPart(p);
        }
    }
}
