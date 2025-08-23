using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace EliteBionicsFrameworkLegacy.Patches
{
    [HarmonyPatch]
    internal class EBF_QualityBionics_GetMaxHealth
    {
        internal static bool IsCorrectRimWorldVersion()
        {
            // only do this for RimWorld 1.5
            var version = RimWorldDetector.RimWorldVersion;
            return version >= new Version(1, 5) && version < new Version(1, 6);
        }

        internal static bool HasCorrectLoadedMod()
        {
            return LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Quality Bionics") && pack.PackageId.Contains("assassinsbro"));
        }

        public static bool Prepare()
        {
            return IsCorrectRimWorldVersion() && HasCorrectLoadedMod();
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("EBF.Patches.Unification.QualityBionicsContinued.PreFix_QualityBionicsContinued_GetMaxHealth:Prepare");
        }

        [HarmonyPostfix]
        public static void DoNotPreventPatching(ref bool __result)
        {
            // do not prevent patching because afaik the method is already gone
            __result = false;
        }
    }
}
