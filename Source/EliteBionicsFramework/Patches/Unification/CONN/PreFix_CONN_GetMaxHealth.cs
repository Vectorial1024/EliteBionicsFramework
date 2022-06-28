using EBF.Util;
using HarmonyLib;
using System.Reflection;

namespace EBF.Patches.Unification.CONN
{
    [HarmonyPatch]
    public class PreFix_CONN_GetMaxHealth
    {
        public static bool Prepare()
        {
            return ModDetector.CONNIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("CONN.BodyPartDef_GetMaxHealthPatch:BodyPartDef_GetMaxHealth_PostFix");
        }

        [HarmonyPrefix]
        public static bool DoNotDoThePatch()
        {
            // we extend the generosity of fixing Quality Bionic's wrong implementation by unifying them under our management
            return false;
        }
    }
}
