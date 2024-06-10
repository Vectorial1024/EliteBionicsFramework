using EBF.Util;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(WorldPawns))]
    [HarmonyPatch(nameof(WorldPawns.Notify_PawnDestroyed), MethodType.Normal)]
    public class PostFix_WorldPawns_PawnDestroyed
    {
        [HarmonyPostfix]
        public static void OnPawnDestroyed(Pawn p)
        {
            MaxHealthCache.ResetCacheForPawn(p);
            ToolPowerInfoCache.ResetCacheForPawn(p);
        }
    }
}
