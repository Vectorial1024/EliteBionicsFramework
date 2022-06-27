using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EBF.Hediffs
{
    public class HediffCompProperties_MaxHPAdjust_Fake : HediffCompProperties_MaxHPAdjust
    {
        public String providerNamespace;
        public bool isPriiority;

        public HediffCompProperties_MaxHPAdjust_Fake()
        {
            // this is not supposed to be connected to the game directly, it is supposed to be fake
            compClass = null;
        }

        public override string ProviderNamespaceString
        {
            get
            {
                return providerNamespace;
            }
        }

        public override bool IsPriority
        {
            get
            {
                return isPriiority;
            }
        }
    }
}
