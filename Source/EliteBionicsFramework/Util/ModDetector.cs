using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Util
{
    public class ModDetector
    {
        internal static string PackageIdCyberFaunaOfficial = "Daniledman.CyberFauna";

        internal static string PackageIdMechalitCoreOfficial = "Daniledman.MechalitCore";

        internal static IEnumerable<ModContentPack> RunningValidMods = LoadedModManager.RunningMods.Where((ModContentPack pack) => pack != null && pack.AnyContentLoaded());

        public static bool PawnmorpherIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("Pawnmorpher"));

        public static bool PrepareCarefullyIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("EdB Prepare Carefully"));

        public static bool CalloutsIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("Callouts"));

        public static bool MoodyIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("Moody"));

        public static bool QualityBionicsIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("Quality Bionics"));

        public static bool CONNIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("Cybernetic Organism"));

        public static bool CyberFaunaIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("Cyber Fauna"));

        public static bool CyberFaunaOfficialIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.PackageId == PackageIdCyberFaunaOfficial.ToLower());

        public static bool MechalitCoreIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("Mechalit Core"));

        public static bool MechalitCoreOfficialIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.PackageId == PackageIdMechalitCoreOfficial.ToLower());

        public static bool HalfDragonsIsLoaded => RunningValidMods.Any((ModContentPack pack) => pack.Name.Contains("Half dragons"));
    }
}
