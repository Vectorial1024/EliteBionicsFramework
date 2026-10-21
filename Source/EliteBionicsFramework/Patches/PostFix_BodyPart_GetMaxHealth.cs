using HarmonyLib;
using System;
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
        /// Because RimWorld is *mostly* single-threaded (limited peripheral multithreading as of RW 1.6), we can do crazy things like this.
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
        public static void CheckEbfProtocolViolation()
        {
            if (warningIsTurnedOff)
            {
                // sometimes, we just need the system to shut up and trust the process...
                return;
            }

            // change of strategy: if we are no longer preventing the vanilla usage, might as well do it in the post-fix.
            // we will also scan for an appropriate caller;.
            // as of RimWorld 1.7 (Waylith Engine), transpiled methods are no longer assigned the `MonoMod.Utils.DynamicMethodDefinition` type, but are true null types.
            // eventually, we should be able to reach a "solid" caller so to speak, but that's not guaranteed.
            
            // level 0 is here, so we start with level 2
            // level 1 is presumed/known to be the dynamic method that is stitched together from the GetMaxHealth patches, so it will call this to check violation
            int stackLevel = 2;
            string namespaceString = null;
            while (stackLevel <= 10)
            {
                StackFrame investigateFrame = new StackFrame(stackLevel);
                Type callerType = investigateFrame.GetMethod()?.DeclaringType;
                if (callerType != null)
                {
                    string innerNamespace = callerType.Namespace ?? null;
                    // regression: also check DynamicMethodDefinition for RimWorld 1.6!
                    // todo delete this when have separate DLLs for RimWorld 1.7 or later
                    string callerFullName = callerType.FullName;
                    if (callerFullName != "MonoMod.Utils.DynamicMethodDefinition")
                    {
                        namespaceString = innerNamespace;
                        break;
                    }
                }
                // check the next level
                stackLevel++;
            }
            bool logIt = true;
            if (namespaceString == null)
            {
                logIt = false;
                namespaceString = "(unknown; check stacktrace!)";
            }

            if (shouldSupressNextWarning)
            {
                shouldSupressNextWarning = false;
            }
            else if (logIt && !reportedNamespaces.Contains(namespaceString))
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
