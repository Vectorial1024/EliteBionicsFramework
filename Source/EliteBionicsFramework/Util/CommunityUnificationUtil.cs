using EBF.Hediffs;
using System;
using System.Reflection;
using System.Text;
using Verse;

namespace EBF.Util
{
    public class CommunityUnificationUtil
    {
        public static String GetCompTipStringExtraDueToMaxHpAdjust(Pawn pawn, BodyPartDef def, HediffCompProperties_MaxHPAdjust props)
        {
            StringBuilder builder = new StringBuilder("");

            // Base HP tooltip
            float rawMaxHP = def.GetRawMaxHealth(pawn);
            builder.Append("Base HP: ");
            builder.Append(rawMaxHP);

            // Scale adjustment tooltip
            if (props.scaleAdjustment != 0)
            {
                builder.AppendLine();
                builder.Append("Max HP: ");
                builder.Append(props.ScaledAdjustmentDisplayString);
            }

            // Linear adjustment tooltip
            if (props.linearAdjustment != 0)
            {
                builder.AppendLine();
                builder.Append("Max HP: ");
                builder.Append(props.LinearAdjustmentDisplayString);
            }

            // Provider tooltip
            builder.AppendLine();
            builder.Append("Provided via: ");
            builder.Append(props.ProviderNamespaceString);

            return builder.ToString();
        }
    }
}
