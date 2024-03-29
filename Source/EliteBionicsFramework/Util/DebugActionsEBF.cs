﻿using System.Text;
using Verse;

namespace EBF.Util
{
    public static class DebugActionsEBF
    {
        [DebugAction("Elite Bionics Framework", name = "Dump mod detection info")]
        private static void DumpModDetectionInfo()
        {
            StringBuilder builder = new StringBuilder("Elite Bionics Framework: mod detection info");
            builder.AppendInNewLine("-----------------------");

            // "foreach" the mod detection info
            builder.AppendInNewLine($"Pawnmorpher: {ModDetector.PawnmorpherIsLoaded}");
            builder.AppendInNewLine($"Prepare Carefully: {ModDetector.PrepareCarefullyIsLoaded}");
            builder.AppendInNewLine($"Callouts: {ModDetector.CalloutsIsLoaded}");
            builder.AppendInNewLine($"Moody: {ModDetector.MoodyIsLoaded}");
            builder.AppendInNewLine($"Quality Bionics: {ModDetector.QualityBionicsIsLoaded}");
            builder.AppendInNewLine($"Cybernetic Organisms and Neural Networks (CONN): {ModDetector.CONNIsLoaded}");
            builder.AppendInNewLine($"Cyber Fauna (official): {ModDetector.CyberFaunaOfficialIsLoaded}");
            builder.AppendInNewLine($"Mechalit Core (official): {ModDetector.MechalitCoreIsLoaded}");
            builder.AppendInNewLine($"Half Dragons: {ModDetector.HalfDragonsIsLoaded}");

            builder.AppendInNewLine("-----------------------");
            builder.AppendInNewLine("All relevant mods checked.");

            Log.Error(builder.ToString());
        }
    }
}
