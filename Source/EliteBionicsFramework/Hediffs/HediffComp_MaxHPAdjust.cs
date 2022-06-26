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
                if (EliteBionicsFrameworkMain.SettingHandle_DisplayHpDiffInHediffName.Value)
                {
                    StringBuilder builder = new StringBuilder("HP: ");
                    StringBuilder innerBuilder = new StringBuilder();
                    if (Props.scaleAdjustment != 0)
                    {
                        innerBuilder.Append(Props.ScaledAdjustmentDisplayString);
                    }
                    if (Props.linearAdjustment != 0)
                    {
                        if (innerBuilder.Length > 0)
                        {
                            innerBuilder.Append(", ");
                        }
                        innerBuilder.Append(Props.LinearAdjustmentDisplayString);
                    }

                    if (innerBuilder.Length > 0) 
                    {
                        builder.Append(innerBuilder.ToString());
                        return builder.ToString();
                    }
                    // nothing to display
                    return "";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
