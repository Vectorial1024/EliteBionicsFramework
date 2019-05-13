using Harmony;
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
        [HarmonyPrefix]
        public static bool PreFix(BodyPartDef __instance, float __result, Pawn pawn)
        {
            if (!EliteBionicsFrameworkMain.MoodyIsRunning)
            {
                // The StackTrace looks something like this:
                // (this method)
                // BodyPartDef GetMaxHealth(...)
                // Moody.* *
                // So it is at frame 2
                StackFrame determinantFrame = new StackFrame(2);
                if (determinantFrame.GetMethod().ReflectedType.Namespace.Contains("Moody"))
                {
                    EliteBionicsFrameworkMain.MoodyIsRunning = true;
                    string message = "Elite Bionics Framework has detected Moody using the vanilla " +
                        "method of BodyPartDef.GetMaxHealth(). While this violation will not cause " +
                        "the game to crash, it will cause Moody to print out negative HP for some " +
                        "body parts.\n" +
                        "Due to technical limitations, EBF is unable to resolve this error. Contact " +
                        "the author(s) of Moody to see if they are willing to resolve this violation.";
                    EliteBionicsFrameworkMain.LogError(message);
                }
            }
            if (!EliteBionicsFrameworkMain.MoodyIsRunning)
            {
                string message = "Elite Bionics Framework has detected some other mod(s) " +
                    "using the vanilla method of BodyPartDef.GetMaxHealth(). While this " +
                    "violation will not cause the game to crash, it will cause those other " +
                    "mod(s) to behave unexpectedly.\n" +
                    "Report to Vectorial1024 and the author(s) of those mod(s) to see " +
                    "if those violations can be solved.\n";
                EliteBionicsFrameworkMain.LogError(message + Environment.StackTrace, true);
            }
            __result = __instance.GetRawMaxHealth(pawn);
            return false;
        }
    }
}
