using EBF;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EliteBionicsFramework.Patches
{
    [HarmonyPatch(typeof(Hediff_MissingPart))]
    [HarmonyPatch("BleedRate", MethodType.Getter)]
    public class PostFix_HediffMissingPart_BleedRate
    {
        [HarmonyPostfix]
        public static void PostFix(Hediff_MissingPart __instance, ref float __result)
        {
            if (__result > 0)
            {
                // Scales it down by (Real HP : Displayed HP)
                __result = __result * __instance.Part.def.GetRawMaxHealth(__instance.pawn) / __instance.Part.def.GetMaxHealth(__instance.pawn, __instance.Part);
            }
        }
    }
}
