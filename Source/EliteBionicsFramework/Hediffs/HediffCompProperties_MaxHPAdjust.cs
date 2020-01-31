using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EBF.Hediffs
{
    public class HediffCompProperties_MaxHPAdjust : HediffCompProperties
    {
        public int linearAdjustment;
        public float scaleAdjustment;

        public HediffCompProperties_MaxHPAdjust()
        {
            compClass = typeof(HediffComp_MaxHPAdjust);
        }
    }
}
