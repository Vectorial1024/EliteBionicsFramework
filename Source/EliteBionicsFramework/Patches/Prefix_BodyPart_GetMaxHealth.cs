using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EliteBionicsFramework.Patches
{
    [HarmonyPatch(typeof(BodyPartDef))]
    [HarmonyPatch("GetMaxHealth", MethodType.Normal)]
    public class Prefix_BodyPart_GetMaxHealth
    {
        [HarmonyPrefix]
        public static bool PreFix(BodyPartDef __instance, float __result, Pawn pawn)
        {
            EliteBionicsFrameworkMain.LogError("Some mod(s) did not comply with our Elite Bionics Framework standard. Notify Vectorial1024 and their authors.\n" + Environment.StackTrace, true);
            __result = __instance.GetRawMaxHealth(pawn);
            return false;
        }
    }
}
