using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(BodyPartDef))]
    [HarmonyPatch(nameof(BodyPartDef.GetMaxHealth), MethodType.Normal)]
    public class PostFix_BodyPart_GetMaxHealth
    {
        private static List<string> reportedNamespaces = new List<string>();

        internal static bool shouldSupressNextWarning = false;

        internal static bool warningIsTurnedOff = false;

        /// <summary>
        /// Suppresses the next "EBF Protocol violation" error message.
        /// Because RimWorld is only single-threaded, we can do crazy things like this.
        /// </summary>
        internal static void SuppressNextWarning()
        {
            shouldSupressNextWarning = true;
        }

        /// <summary>
        /// Temporarily turns off the "EBF Protocol violation" error message until it is turned back on.
        /// Avoid using this way of error suppression where possible.
        /// </summary>
        internal static void TurnOffWarning()
        {
            warningIsTurnedOff = true;
        }

        /// <summary>
        /// Resumes normal "EBF Protocol violation" error message.
        /// Avoid using this way of error suppression where possible.
        /// </summary>
        internal static void TurnOnWarning()
        {
            warningIsTurnedOff = false;
        }

        [HarmonyPostfix]
        public static void CheckEbfProtocolViolation(BodyPartDef __instance, ref float __result, Pawn pawn)
        {
            if (warningIsTurnedOff)
            {
                // sometimes, we just need the system to shut up and trust the process...
                return;
            }

            // change of strategy: if we are no longer preventing the vanilla usage, might as well do it in the post-fix.
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
                EliteBionicsFrameworkMod.LogError(errorMessage);
            }
            return;
        }
    }
}
