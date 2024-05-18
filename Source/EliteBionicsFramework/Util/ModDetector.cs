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

        public static bool QualityBionicsIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Quality Bionics"));

        public static bool CONNIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Cybernetic Organism"));

        public static bool CyberFaunaIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Cyber Fauna"));

        public static bool CyberFaunaOfficialIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.PackageId == PackageIdCyberFaunaOfficial.ToLower());

        public static bool MechalitCoreIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Mechalit Core"));

        public static bool MechalitCoreOfficialIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.PackageId == PackageIdMechalitCoreOfficial.ToLower());

        public static bool HalfDragonsIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Half dragons"));

        /// <summary>
        /// Determines whether the DLL "ProthesisHealth" has been loaded.
        /// <para/>
        /// At this moment, both Cyber Fauna and Mechalit Core utilizes this dll, and a true value here indicates that at least one of the two mods are loaded. 
        /// Whichever one is loaded, this you should ask in another query.
        /// </summary>
        public static bool DllProthesisHealthisLoaded => CyberFaunaOfficialIsLoaded || MechalitCoreOfficialIsLoaded;

        public static bool VanillaPsycastsExpandedIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Vanilla Psycasts Expanded"));

        public static bool ImmortalsIsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Immortals"));

        public static bool SaveOurShip2IsLoaded => RunningActiveMods.Any((ModContentPack pack) => pack.Name.Contains("Save Our Ship 2"));
    }
}
