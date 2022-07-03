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

        public static bool PawnmorpherIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Pawnmorpher"));

        public static bool PrepareCarefullyIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("EdB Prepare Carefully"));

        public static bool CalloutsIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Callouts"));

        public static bool MoodyIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Moody"));

        public static bool QualityBionicsIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Quality Bionics"));

        public static bool CONNIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Cybernetic Organism"));

        public static bool CyberFaunaIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Cyber Fauna"));

        public static bool CyberFaunaOfficialIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.PackageId.Equals(PackageIdCyberFaunaOfficial));
    }
}
