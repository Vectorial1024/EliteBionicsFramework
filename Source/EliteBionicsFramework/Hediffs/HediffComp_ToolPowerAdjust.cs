using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Hediffs
{
    class HediffComp_ToolPowerAdjust: HediffComp
    {
        public HediffCompProperties_ToolPowerAdjust Props => (HediffCompProperties_ToolPowerAdjust)props;

        public override string CompTipStringExtra
        {
            get
            {
                StringBuilder builder = new StringBuilder("");

                // Base verb power tooltip
                /*
                float rawMaxHP = parent.Part.def.GetRawMaxHealth(parent.pawn);
                builder.Append("Base HP: ");
                builder.Append(rawMaxHP);
                */

                // Scale adjustment tooltip
                if (Props.scaleAdjustment != 0)
                {
                    builder.AppendLine();
                    builder.Append("Verb power: ");
                    builder.Append(Props.ScaledAdjustmentDisplayString);
                }

                // Linear adjustment tooltip
                if (Props.linearAdjustment != 0)
                {
                    builder.AppendLine();
                    builder.Append("Verb power: ");
                    builder.Append(Props.LinearAdjustmentDisplayString);
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
                    StringBuilder builder = new StringBuilder("HP: ");
                    if (Props.scaleAdjustment != 0)
                    {
                        builder.Append(Props.ScaledAdjustmentDisplayString);
                    }
                    if (Props.linearAdjustment != 0)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(Props.LinearAdjustmentDisplayString);
                    }

                    return builder.ToString();
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
