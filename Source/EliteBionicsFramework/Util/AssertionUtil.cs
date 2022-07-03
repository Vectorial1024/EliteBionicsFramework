using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Util
{
    public class AssertionUtil
    {
        /// <summary>
        /// we can't really "assert" in RimWorld let alone in C#.
        /// however we can pretend to assert: we can perform "assertions" during game load and show errors when they fail.
        /// we have no power of stopping the player from playing the game, though.
        /// </summary>
        public static void GameStartAssertion()
        {
            // check Cyber Fauna
            bool hasError = false;
            hasError = AssertCyberFauna();
            if (hasError)
            {
                Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
            }
        }

        private static bool AssertCyberFauna()
        {
            if (!ModDetector.CyberFaunaIsLoaded)
            {
                return false;
            }
            if (ModDetector.CyberFaunaIsLoaded && !ModDetector.CyberFaunaOfficialIsLoaded)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(EliteBionicsFrameworkMain.MODPREFIX);
                builder.AppendLine("CHECK FAILED; REASON:");
                builder.AppendLine();
                builder.AppendLine("You are using Cyber Fauna, and we do support Cyber Fauna. But, you are using a version of Cyber Fauna that we do not support.");
                builder.AppendLine();
                builder.AppendLine("We expect to find Cyber Fauna (Mod ID " + ModDetector.PackageIdCyberFaunaOfficial + "; Steam ID 1548649032).");
                builder.AppendLine();
                builder.AppendLine("You should double check your mod list.");
                MakeNewAssertionMessageBox(builder.ToString());
                return true;
            }
            return false;
        }

        private static void MakeNewAssertionMessageBox(string text)
        {
            Find.WindowStack.Add(new Dialog_MessageBox(text, buttonADestructive: true));
        }
    }
}
