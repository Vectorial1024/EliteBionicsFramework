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
                StringBuilder builder = new StringBuilder("");
                if (Props.scaleAdjustment != 0)
                {
                    builder.AppendLine("Max HP: ×" + ((int) ((Props.scaleAdjustment + 1) * 100)).ToStringCached() + "%");
                }
                if (Props.linearAdjustment != 0)
                {
                    builder.AppendLine("Max HP: +" + Props.linearAdjustment.ToStringCached() + " HP");
                }
                return builder.ToString();
            }
        }
        
        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (EliteBionicsFrameworkMain.SettingHandle_DisplayHpDiffInHediffName.Value)
                {
                    return "adjusts max HP";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
