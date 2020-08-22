using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(BodyPartDef))]
    [HarmonyPatch("GetMaxHealth", MethodType.Normal)]
    public class Prefix_BodyPart_GetMaxHealth
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

        [HarmonyPrefix]
        public static bool PreFix(BodyPartDef __instance, float __result, Pawn pawn)
        {
            StackFrame investigateFrame = new StackFrame(2);
            string namespaceString = investigateFrame.GetMethod().ReflectedType.Namespace;
            
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
            
            return true;
        }
    }
}
