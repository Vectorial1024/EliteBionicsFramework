using EBF.Transpilations;
using EBF.Transpilations.Pawnmorpher;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace EBF
{
    public class EliteBionicsFrameworkMod : Mod
    {
        public static string MODSHORTID => "V1024-EBF";

        public static FrameworkSettings Settings { get; private set; }

        public EliteBionicsFrameworkMod(ModContentPack content) : base(content)
        {
            // since we no longer depend on HugsLib, we have to apply the harmony patches ourselves
            LogInfo("Elite Bionics Framework, starting up. Hopefully the patches work.");
            Harmony harmony = new Harmony("rimworld." + content.PackageId);
            harmony.PatchAll();

            // still, one thing we must do first: touch the blunt patch
            // this is in response to Yayo's Animations doing their patches this early, and is an unfortunate conclusion of events.
            LogInfo("Precautionary super-early patching of DamageWorker_Blunt. This is to handle a known edge-case. ");
            Transpiler_DamageWorker_Blunt_SpecialEffects.TranspileTheTarget(null);
            Transpiler_DamageWorker_MutagenicBlunt_SpecialEffects.TranspileTheTarget(null);

            // we also need to read the mod settings ourselves
            Settings = GetSettings<FrameworkSettings>();
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
        public override string SettingsCategory()
        {
            return "Elite Bionics Framework";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}
