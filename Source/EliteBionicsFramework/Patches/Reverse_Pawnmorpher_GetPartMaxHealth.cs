using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch]
    public class Reverse_Pawnmorpher_GetPartMaxHealth
    {
        public static bool Prepare()
        {
            return ModDetector.PawnmorpherIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.BodyUtilities:GetPartMaxHealth");
        }

        [HarmonyReversePatch]
        public static float GetPartMaxHealth(BodyPartRecord record, Pawn p)
        {
            throw new NotImplementedException("Called a stub before reverse patching is complete.");
        }
    }
}
