using EBF.Transpilations;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF
{
    public class EliteBionicsFrameworkMod : Mod
    {
        public EliteBionicsFrameworkMod(ModContentPack content) : base(content)
        {
            // we leave the other patches to HugsLib, but one thing we must do first: touch the blunt patch
            // this is in response to Yayo's Animations doing their patches this early, and is an unfortunate conclusion of events.
            Log.Message(EliteBionicsFrameworkMain.MODPREFIX + "Super-early patching of DamageWorker_Blunt. This is to handle a known edge-case. " +
                "Remaining patches of this mod will still be done at the usual, appropriate moments.");
            Transpiler_DamageWorker_Blunt_SpecialEffects.Transpiler(new List<CodeInstruction>());
        }
    }
}
