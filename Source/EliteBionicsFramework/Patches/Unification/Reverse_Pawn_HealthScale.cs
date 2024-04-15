using HarmonyLib;
using System;
using Verse;

namespace EBF.Patches.Unification
{
    /// <summary>
    /// This is used to detect whether the health-scale of a pawn has been modified by some other mods.
    /// </summary>
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(nameof(Pawn.HealthScale), MethodType.Getter)]
    public class Reverse_Pawn_HealthScale
    {
        [HarmonyReversePatch]
        public static float GetOriginalHealthScale(Pawn __instance)
        {
            throw new NotImplementedException("Called a stub before reverse patching is complete.");
        }
    }
}
