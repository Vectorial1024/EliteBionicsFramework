using System.Text;
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
            hasError |= AssertCyberFauna();
            hasError |= AssertMechalitCore();
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

            StringBuilder builder = new StringBuilder();
            builder.Append(EliteBionicsFrameworkMain.MODPREFIX);
            builder.AppendLine("CHECK FAILED; REASON:");
            builder.AppendLine();
            builder.AppendLine("You are using Cyber Fauna, but unfortunately, we do not feel comfortable supporting Cyber Fauna.");
            builder.AppendLine();
            builder.AppendLine("You should consider disusing it, or find alternatives to it.");
            MakeNewAssertionMessageBox(builder.ToString());
            return true;
        }

        private static bool AssertMechalitCore()
        {
            if (!ModDetector.MechalitCoreIsLoaded)
            {
                return false;
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(EliteBionicsFrameworkMain.MODPREFIX);
            builder.AppendLine("CHECK FAILED; REASON:");
            builder.AppendLine();
            builder.AppendLine("You are using Mechalit Core, but unfortunately, we do not feel comfortable supporting Mechalit Core.");
            builder.AppendLine();
            builder.AppendLine("You should consider disusing it, or find alternatives to it.");
            MakeNewAssertionMessageBox(builder.ToString());
            return true;
        }

        private static void MakeNewAssertionMessageBox(string text)
        {
            Find.WindowStack.Add(new Dialog_MessageBox(text, buttonADestructive: true));
        }
    }
}
