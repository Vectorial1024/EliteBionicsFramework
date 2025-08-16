using EBF.API;
using EBF.Hediffs;
using Verse;

namespace EBF.Extensions
{
    internal static class SelfExtender
    {
        public static bool TryExtractEbfExternalCompProps(this HediffComp hediffComp, out HediffCompProperties_MaxHPAdjust_Fake fakeComp)
        {
            fakeComp = null;
            if (hediffComp is IHediffCompAdjustsMaxHp ebfExternalComp)
            {
                var adjustment = ebfExternalComp.MaxHpAdjustment;
                fakeComp = new HediffCompProperties_MaxHPAdjust_Fake
                {
                    linearAdjustment = adjustment.LinearAdjustment,
                    scaleAdjustment = adjustment.ScaleMultiplier - 1,
                    providerNamespace = adjustment.EffectProvider,
                };
                return true;
            }
            return false;
        }
    }
}
