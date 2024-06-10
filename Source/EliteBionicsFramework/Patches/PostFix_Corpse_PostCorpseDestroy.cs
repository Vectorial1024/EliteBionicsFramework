using EBF.Util;
using HarmonyLib;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(Corpse))]
    [HarmonyPatch(nameof(Corpse.PostCorpseDestroy), MethodType.Normal)]
    public class PostFix_Corpse_PostCorpseDestroy
    {
        [HarmonyPostfix]
        public static void OnCorpseDestroyed(Pawn pawn)
        {
            MaxHealthCache.ResetCacheForPawn(pawn);
            ToolPowerInfoCache.ResetCacheForPawn(pawn);
        }
    }
}
