using EBF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EBF.Hediffs
{
    public class HediffComp_MaxHPAdjust: HediffComp
    {
        public HediffCompProperties_MaxHPAdjust Props => (HediffCompProperties_MaxHPAdjust)props;

        // private static readonly Texture2D IconMaxHPStrengthened = ContentFinder<Texture2D>.Get("UI/Icons/Medical/EBF_MHP_Strengthen", true);

        public override string CompTipStringExtra
        {
            get
            {
                return CommunityUnificationUtil.GetCompTipStringExtraDueToMaxHpAdjust(Pawn, parent.Part.def, Props);
            }
        }
        
        public override string CompLabelInBracketsExtra
        {
            get
            {
                return CommunityUnificationUtil.GetCompLabelInBracketsDueToMaxHpAdjust(Pawn, parent);
            }
        }
    }
}
