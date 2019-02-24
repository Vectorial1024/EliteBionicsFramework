using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EliteBionicsFramework.Hediffs
{
    public class HediffCompProperties_MaxHPAdjust : HediffCompProperties
    {
        public int linearAdjustment;

        public HediffCompProperties_MaxHPAdjust()
        {
            compClass = typeof(HediffComp_MaxHPAdjust);
        }
    }
}
