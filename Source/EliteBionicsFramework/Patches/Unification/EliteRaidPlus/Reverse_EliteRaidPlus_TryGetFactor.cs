using EBF.Util;
using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace EBF.Patches.Unification.EliteRaidPlus
{
    [HarmonyPatch]
    public class Reverse_EliteRaidPlus_TryGetFactor
    {
        public static bool Prepare()
        {
            // patch deprecated, but we retain code for future reference
            return ModDetector.EliteRaidPlusIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("EliteRaidPlus.BodyPartHealthFactorCache:TryGetFactor");
        }

        [HarmonyReversePatch]
        public static bool TryGetFactor(Pawn pawn, out float factor)
        {
            throw new NotImplementedException("Called a stub before reverse patching is complete.");
        }
    }
}
