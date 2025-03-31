using EBF.Util;
using HarmonyLib;
using System.Reflection;

namespace EBF.Patches.AlteredCarbon
{
    [HarmonyPatch]
    public class Mixed_AC2_SleeveGestator_PutCorpse
    {
        /*
         * Note to self:
         * This case is very difficult to patch in the usual way because the compiler essentially in-lined the lambda function call into some slippery byte code.
         * Luckily, because this case involves only Hediff_MissingPart, EBF has no actual need to be active here.
         * Therefore, we are using another approach: simply suppress EBF and call it a day.
         */

        public static bool Prepare()
        {
            return ModDetector.AlteredCarbon2IsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("AlteredCarbon.Building_SleeveGestator:PutCorpseForRepurposing");
        }

        [HarmonyPrefix]
        public static void BeginIgnoreEbf()
        {
            PostFix_BodyPart_GetMaxHealth.TurnOffWarning();
        }

        [HarmonyPostfix]
        public static void EndIgnoreEbf()
        {
            PostFix_BodyPart_GetMaxHealth.TurnOnWarning();
        }
    }
}
