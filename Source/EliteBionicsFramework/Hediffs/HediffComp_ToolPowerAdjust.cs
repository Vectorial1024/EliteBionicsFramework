using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Hediffs
{
    public class HediffComp_ToolPowerAdjust: HediffComp
    {
        public HediffCompProperties_ToolPowerAdjust Props => (HediffCompProperties_ToolPowerAdjust)props;

        public override string CompTipStringExtra
        {
            get
            {
                StringBuilder builder = new StringBuilder("");

                // Base verb power tooltip
                // note: since there can be multiple possible tools that we are adjusting, we can only provide vague phrases here.
                if (parent is Hediff_Implant && parent is not Hediff_AddedPart)
                {
                    // hediff-implant respects the existing base verb power
                    builder.Append("Adjusting existing verb power");
                }
                else
                {
                    // other cases attempt to use original (base) power for adjustment
                    builder.Append("Adjusting original verb power");
                }

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
                    StringBuilder builder = new StringBuilder("Verb power: ");
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
