using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace EliteBionicsFrameworkLegacy.Patches
{
    [HarmonyPatch]
    internal class EBF_CalloutsRedirect
    {
        internal static bool IsCorrectRimWorldVersion()
        {
            // only do this for RimWorld 1.3 to 1.5 inclusive
            // as of 1.6, Callouts (continued by Mlie) has integration with Elite Bionics Framework
            var version = RimWorldDetector.RimWorldVersion;
            return version >= new Version(1, 3) && version < new Version(1, 6);
        }

        public static bool Prepare()
        {
            return IsCorrectRimWorldVersion() && TargetMethod() != null;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.PropertyGetter("EBF.Util.ModDetector:CalloutsIsLoaded"); ;
        }

        [HarmonyPostfix]
        public static void DoNotDetectMlieVariant(ref bool __result)
        {
            // do not detect Mlie's variant because they have integrated support for Elite Bionics Framework
            if (__result)
            {
                __result &= !LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.PackageId == "Mlie.Callouts");
            }
        }
    }
}
