using HarmonyLib;
using Verse;
using static Verse.PawnCapacityUtility;

namespace EBF.Patches
{
    [HarmonyPatch(typeof(CapacityImpactorBodyPartHealth))]
    [HarmonyPatch(nameof(CapacityImpactorBodyPartHealth.Readable), MethodType.Normal)]
    public class PreFix_PawnCapUtil_BodyPartHealth
    {
        [HarmonyPrefix]
        public static bool PreFix(CapacityImpactorBodyPartHealth __instance, ref string __result, Pawn pawn)
        {
            HediffSet hediffSet = pawn.health.hediffSet;
            BodyPartRecord record = __instance.bodyPart;
            __result = $"{record.LabelCap}: {hediffSet.GetPartHealth(record)} / {hediffSet.GetPartMaxHealth(record)}";
            return false;
        }
    }
}
