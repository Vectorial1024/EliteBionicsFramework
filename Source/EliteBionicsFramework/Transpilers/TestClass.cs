using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using static Verse.DamageWorker;

namespace EliteBionicsFramework.Transpilations
{
    class TestClass
    {
        private void testMethod(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
        {
            float x3 = totalDamage / pawn.GetCorePartMaxHealth();
        }
    }
}
