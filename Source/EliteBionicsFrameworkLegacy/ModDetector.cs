using System.Linq;
using Verse;

namespace EliteBionicsFrameworkLegacy
{
    public class ModDetector
    {
        public static bool QualityBionicsRemasteredIsLoaded => LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Quality Bionics") && pack.PackageId.Contains("assassinsbro"));
    }
}
