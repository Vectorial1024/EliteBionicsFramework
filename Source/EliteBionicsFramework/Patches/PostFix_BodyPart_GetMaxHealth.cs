using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(BodyPartDef))]
    [HarmonyPatch(nameof(BodyPartDef.GetMaxHealth), MethodType.Normal)]
    public class PostFix_BodyPart_GetMaxHealth
    {
        private static readonly HashSet<string> reportedNamespaces = new HashSet<string>();

        [ThreadStatic]
        internal static bool shouldSupressNextWarning = false;

        [ThreadStatic]
        internal static bool warningIsTurnedOff = false;

        /// <summary>
        /// Suppresses the next "EBF Protocol violation" error message.
        /// The next inquiry on this thread consumes the permission when it starts.
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

        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static void CaptureWarningSuppression(out bool __state)
        {
            // Capture permission before other patches can make nested inquiries. A postfix
            // must not consume permission intended for a different invocation or thread.
            __state = warningIsTurnedOff || shouldSupressNextWarning;
            shouldSupressNextWarning = false;
        }

        [HarmonyPostfix]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CheckEbfProtocolViolation(BodyPartDef __instance, ref float __result, Pawn pawn, bool __state)
        {
            if (__state)
            {
                return;
            }

            string namespaceString = GetCallerNamespace(new StackTrace(1, false));
            lock (reportedNamespaces)
            {
                if (!reportedNamespaces.Add(namespaceString))
                {
                    return;
                }
            }

            string errorMessage = "Elite Bionics Framework has detected some mods" +
            " using the unmodified GetMaxHealth() method, which violates the" +
            " EBF protocol. The author(s) of the involved mod(s) should " +
            "adopt the EBF to clarify their intentions.\n" +
            "For now, the unmodified max HP is returned.\n" +
            "The detected mod comes from: " + namespaceString;
            EliteBionicsFrameworkMod.LogError(errorMessage);
        }

        private static string GetCallerNamespace(StackTrace trace)
        {
            foreach (StackFrame frame in trace.GetFrames() ?? Array.Empty<StackFrame>())
            {
                // Harmony replacements are DynamicMethods on CoreCLR and do not have a
                // reflected type. Resolve them to the original method before attribution.
                var caller = Harmony.GetOriginalMethodFromStackframe(frame) ?? frame.GetMethod();
                Type callerType = caller?.DeclaringType;
                if (callerType == null || callerType == typeof(PostFix_BodyPart_GetMaxHealth))
                {
                    continue;
                }
                if (callerType == typeof(BodyPartDef) && caller.Name == nameof(BodyPartDef.GetMaxHealth))
                {
                    continue;
                }
                return callerType.Namespace ?? callerType.Assembly.GetName().Name;
            }

            // Preserve a once-only diagnostic if a runtime cannot recover any caller.
            return "<unresolved caller>";
        }
    }
}
