using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("LoadGame")]
    public class PostFix_Game_LoadGame
    {
        [HarmonyPostfix]
        public static void PostFix()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                AssertionUtil.GameStartAssertion();
            });
        }
    }
}
