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

        /// <summary>
        /// Suppresses the next "EBF Protocol violation" error message.
        /// Because RimWorld is only single-threaded, we can do crazy things like this.
        /// </summary>
        internal static void SuppressNextWarning()
        {
            shouldSupressNextWarning = true;
        }

        [HarmonyPostfix]
        public static void CheckEbfProtocolViolation(BodyPartDef __instance, ref float __result, Pawn pawn)
        {
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
                EliteBionicsFrameworkMain.LogError(errorMessage);
            }
            return;
        }
    }
}
