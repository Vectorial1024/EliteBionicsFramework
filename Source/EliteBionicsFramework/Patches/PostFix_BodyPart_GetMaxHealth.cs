using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(BodyPartDef))]
    [HarmonyPatch("GetMaxHealth", MethodType.Normal)]
    public class PostFix_BodyPart_GetMaxHealth
    {
        private static List<string> reportedNamespaces = new List<string>();

        internal static bool shouldSupressNextWarning = false;

        /// <summary>
        /// Suppresses the next "EBF Protocol violation" error message.
        /// Because RimWorld is only single-threaded, we can do crazy things like this.
        /// </summary>
        internal static void SuppressNextWarning()
        {
            shouldSupressNextWarning = true;
        }

        [HarmonyPostfix]
        public static void LogEbfProtocolViolation(BodyPartDef __instance, ref float __result, Pawn pawn)
        {
            // change of strategy: if we are no longer preventing the vanilla usage, might as well do it in the post-fix.
            StackFrame investigateFrame = new StackFrame(2);
            string namespaceString = investigateFrame.GetMethod().ReflectedType.Namespace;
            /*
            Log.Error("SanCheck: agetracker " + pawn.ageTracker);
            Log.Error("SanCheck: lifestage " + pawn.ageTracker?.CurLifeStage);
            Log.Error("SanCheck: raceprops " + pawn.RaceProps);
            Log.Error("SanCheck: namespace " + namespaceString);
            Log.Error("SanCheck: instance HP " + __instance.hitPoints);
            */

            if (shouldSupressNextWarning)
            {
                shouldSupressNextWarning = false;
            }
            else if (!reportedNamespaces.Contains(namespaceString))
            {
                reportedNamespaces.Add(namespaceString);
                string errorMessage = "Elite Bionics Framework has detected some mods" +
                " using the unmodified GetMaxHealth() method, which violates the" +
                " EBF protocol. The author(s) of the involved mod(s) should " +
                "adopt the EBF to clarify their intentions.\n" +
                "For now, the unmodified max HP is returned.\n" +
                "The detected mod comes from: " + namespaceString;
                EliteBionicsFrameworkMain.LogError(errorMessage);
            }
            
            /*
            // RW 1.5 strange bug: sometimes some base game values are unexpectedly null, and then it won't work.
            // here we try a workaround.
            if (pawn.ageTracker?.CurLifeStage == null || pawn.RaceProps == null)
            {
                EliteBionicsFrameworkMain.LogWarning("CurLifeStage and/or RaceProps for GetMaxHealth unexpectedly null; bypassing original method to provide emergency workaround; this might break other mods.");
                // this is how the formula looks like in RW 1.5
                float lifeStageHealthScaleFactor = pawn.ageTracker?.CurLifeStage?.healthScaleFactor ?? 1;
                Log.Error("SanCheck: lifeStageFactor " + lifeStageHealthScaleFactor);
                float racePropsBaseHealthScale = pawn.RaceProps?.baseHealthScale ?? 1;
                if (racePropsBaseHealthScale <= 0)
                {
                    racePropsBaseHealthScale = 1;
                }
                Log.Error("SanCheck: racePropsScale " + racePropsBaseHealthScale);
                __result = Mathf.CeilToInt((float) __instance.hitPoints * (lifeStageHealthScaleFactor * racePropsBaseHealthScale));
                // return false;
                return;
            }
            */
            // return true;
            return;
        }
    }
}
