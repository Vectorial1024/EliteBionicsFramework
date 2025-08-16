using LudeonTK;
using System.Text;
using Verse;

namespace EBF.Util
{
    public static class DebugActionsEBF
    {
        [DebugAction("Elite Bionics Framework", name = "Dump mod detection info")]
        public static void DumpModDetectionInfo()
        {
            StringBuilder builder = new StringBuilder("Elite Bionics Framework: mod detection info");
            builder.AppendInNewLine("-----------------------");

            // "foreach" the mod detection info
            builder.AppendInNewLine($"Pawnmorpher: {ModDetector.PawnmorpherIsLoaded}");
            builder.AppendInNewLine($"Prepare Carefully: {ModDetector.PrepareCarefullyIsLoaded}");
            builder.AppendInNewLine($"Callouts: {ModDetector.CalloutsIsLoaded}");
            builder.AppendInNewLine($"Moody: {ModDetector.MoodyIsLoaded}");
            builder.AppendInNewLine($"Quality Bionics: {ModDetector.QualityBionicsIsLoaded}");
            builder.AppendInNewLine($"Quality Bionics (Continued): {ModDetector.QualityBionicsContinuedIsLoaded}");
            builder.AppendInNewLine($"Cybernetic Organisms and Neural Networks (CONN): {ModDetector.CONNIsLoaded}");
            builder.AppendInNewLine($"Vanilla Psycasts Expanded: {ModDetector.VanillaPsycastsExpandedIsLoaded}");
            builder.AppendInNewLine($"Immortals: {ModDetector.ImmortalsIsLoaded}");

            builder.AppendInNewLine("-----------------------");
            builder.AppendInNewLine("All relevant mods checked.");

            Log.Error(builder.ToString());
        }
    }
}
