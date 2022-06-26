using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EBF.Patches
{
    [HarmonyPatch]
    public class Prefix_Pawnmorpher_ModCompat_CompactHediff_GetPartMaxHealth
    {
        public static bool Prepare()
        {
            return ModDetector.PawnmorpherIsLoaded;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method("Pawnmorph.HPatches.Mods.CompactHediffs.GetPartMaxHealthPatch:Transpiler");
        }

        [HarmonyPrefix]
        public static bool PreFix()
        {
            // dont let pawnmorpher do it; we will read their values and print them out by ourselves
            return false;
        }
    }
}
