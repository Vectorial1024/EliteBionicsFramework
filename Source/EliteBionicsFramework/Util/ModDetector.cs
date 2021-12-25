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
        public static bool PawnmorpherIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Pawnmorpher"));

        public static bool PrepareCarefullyIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Prepare Carefully"));

        public static bool CalloutsIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Callouts"));
    }
}
