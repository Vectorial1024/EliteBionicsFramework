using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EliteBionicsFramework.Hediffs
{
    class HediffCompProperties_ToolPowerAdjust : HediffCompProperties
    {
        /*
         * Self-note: base pawn DPS/tool is defined at Core/Defs/ThingDefs_Races/*
         */

        public int linearAdjustment;
        public float scaleAdjustment;

        public HediffCompProperties_ToolPowerAdjust()
        {
            compClass = typeof(HediffComp_ToolPowerAdjust);
        }

        /// <summary>
        /// Returns a string with the format of "+ ... HP" for linear adjustment;
        /// returns empty string if linear adjustment is 0
        /// </summary>
        public string LinearAdjustmentDisplayString
        {
            get
            {
                if (linearAdjustment == 0)
                {
                    return "";
                }
                StringBuilder builder = new StringBuilder("");
                if (linearAdjustment > 0)
                {
                    builder.Append("+");
                }
                builder.Append(linearAdjustment.ToStringCached());
                return builder.ToString();
            }
        }

        /// <summary>
        /// Returns the actual scaling factor given by the scale-adjustment.
        /// <para/>
        /// This value is validated before returning, and may be directly used in multiplication.
        /// </summary>
        public float ActualScalingFactor
        {
            get
            {
                if (scaleAdjustment == 0)
                {
                    return 1;
                }
                if (scaleAdjustment <= -1)
                {
                    // Value rejected
                    return 1;
                }
                return scaleAdjustment + 1;
            }
        }

        /// <summary>
        /// Returns a string with the format of "× ... %" for scaled adjustment (rounds to nearest %);
        /// returns empty string if scaled adjustment is 0
        /// </summary>
        public string ScaledAdjustmentDisplayString
        {
            get
            {
                float factor = ActualScalingFactor;
                if (factor == 1)
                {
                    return "";
                }
                StringBuilder builder = new StringBuilder("×");
                builder.Append(Mathf.RoundToInt(factor * 100).ToStringCached());
                builder.Append("%");
                return builder.ToString();
            }
        }
    }
}
