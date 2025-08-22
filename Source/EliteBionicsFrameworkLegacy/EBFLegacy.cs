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
        public EBFLegacy(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony("rimworld." + content.PackageId + ".legacy");
            harmony.PatchAll();
            Version rimworldVersion = typeof(Mod).Assembly.GetName().Version;
            Log.Error("EYYY");
        }
    }
}
