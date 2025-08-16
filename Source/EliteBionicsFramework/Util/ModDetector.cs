using System.Collections.Generic;
using System.Linq;
using Verse;

namespace EBF.Util
{
    public class ModDetector
    {
        internal static IEnumerable<ModContentPack> RunningActiveMods = LoadedModManager.RunningMods.Where((ModContentPack pack) => pack != null && pack.ModMetaData.Active);

        /// <summary>
        /// Note: if need to call this repeatedly (eg during regular ticking), then please consider using the cached version instead.
        /// <para/>
        /// Users are now complaining about EBF lag spikes, and we don't want lag spikes to affect the user numbers.
        /// </summary>
        public static bool PawnmorpherIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Pawnmorpher"));

        private static bool? hasPawnmorpher = null;

        public static bool PawnmorpherIsLoadedCached
        {
            get
            {
                if (hasPawnmorpher == null)
                {
                    hasPawnmorpher = PawnmorpherIsLoaded;
                }
                return hasPawnmorpher.Value;
            }
        }

        public static bool PrepareCarefullyIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("EdB Prepare Carefully"));

        public static bool CalloutsIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Callouts"));

        public static bool MoodyIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Moody"));

        public static bool QualityBionicsIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Quality Bionics") && pack.PackageId.Contains("rebelrabbit"));

        // Quality Bionics (Continued) changed some of its namespace so things have become confusing for a while
        public static bool QualityBionicsContinuedIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Quality Bionics") && pack.PackageId.Contains("ilyvion"));

        public static bool CONNIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Cybernetic Organism"));

        public static bool CyberFaunaIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Cyber Fauna"));

        public static bool MechalitCoreIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Mechalit Core"));

        // Half Dragons has been sunset.
        // public static bool HalfDragonsIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Half dragons"));

        public static bool VanillaPsycastsExpandedIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Vanilla Psycasts Expanded"));

        public static bool ImmortalsIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Immortals"));

        public static bool BetterInfoCardIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("BetterInfoCard"));

        public static bool SaveOurShips2IsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Save Our Ships 2"));

        public static bool AlteredCarbon2IsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Altered Carbon 2"));
    }
}
