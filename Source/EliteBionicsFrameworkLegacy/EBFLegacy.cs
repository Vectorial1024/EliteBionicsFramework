using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EliteBionicsFrameworkLegacy
{
    // note: this mod is only a util mod that deals with legacy EBF versions. Ideally outside modders do not need to interact with this mod.
    public class EBFLegacy : Mod
    {
        public static string MODSHORTID => "V1024-EBF-L";

        public EBFLegacy(ModContentPack content) : base(content)
        {
            LogInfo("Elite Bionics Framework Legacy Manager starting up with RimWorld version as " + RimWorldDetector.RimWorldVersion);
            Harmony harmony = new Harmony("rimworld." + content.PackageId + ".legacy");
            harmony.PatchAll();
        }

        public static string MODPREFIX => "[" + MODSHORTID + "]";

        public static void LogError(string message)
        {
            Log.Error(MODPREFIX + " " + message);
        }

        public static void LogWarning(string message)
        {
            Log.Warning(MODPREFIX + " " + message);
        }

        public static void LogInfo(string message)
        {
            Log.Message(MODPREFIX + " " + message);
        }
    }
}
