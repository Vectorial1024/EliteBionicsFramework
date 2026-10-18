using EBF.Util;
using HarmonyLib;
using System.Reflection;
using Verse;

namespace EBF.Patches.Unification.EliteRaidPlus
{
    [HarmonyPatch]
    public class PreFix_EliteRaidPlus_BodyPartMaxHealthPatch
    {
        public static bool Prepare()
        {
            return ModDetector.EliteRaidPlusIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("EliteRaidPlus.Core.Patches.BodyPartMaxHealthPatch:Postfix");
        }

        [HarmonyPrefix]
        public static bool DoNotDoIt()
        {
            // we handle this by community unification; don't do it.
            return false;
        }
    }
}
