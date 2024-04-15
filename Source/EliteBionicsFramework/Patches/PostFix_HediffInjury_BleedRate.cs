﻿using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(Hediff_Injury))]
    [HarmonyPatch(nameof(Hediff_Injury.BleedRate), MethodType.Getter)]
    public class PostFix_HediffInjury_BleedRate
    {
        [HarmonyPostfix]
        public static void PostFix(Hediff_Injury __instance, ref float __result)
        {
            if (__result > 0 && __instance.Part != null)
            {
                // Scales it down by (Real HP : Displayed HP)
                __result = __result * __instance.Part.def.GetRawMaxHealth(__instance.pawn) / __instance.Part.def.GetMaxHealth(__instance.pawn, __instance.Part);
            }
        }
    }
}
