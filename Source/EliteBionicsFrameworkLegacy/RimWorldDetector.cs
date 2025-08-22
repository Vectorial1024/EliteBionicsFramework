using System;
using Verse;

namespace EliteBionicsFrameworkLegacy
{
    internal class RimWorldDetector
    {
        private static Version rimWorldVersion = null;

        public static Version RimWorldVersion
        {
            get
            {
                if (rimWorldVersion == null)
                {
                    rimWorldVersion = typeof(Mod).Assembly.GetName().Version;
                }
                return rimWorldVersion;
            }
        }
    }
}
