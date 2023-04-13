using EBF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EBF.Hediffs
{
    public class HediffCompProperties_MaxHPAdjust : HediffCompProperties
    {
        /// <summary>
        /// THe linear max HP adjustment.
        /// <para/>
        /// Note: remember to call MaxHealthCache.ResetCacheForPawn() when you are manually adjusting this value! We now no longer actively monitor the adjustment values!
        /// </summary>
        public int linearAdjustment;
        /// <summary>
        /// THe scaling max HP adjustment.
        /// <para/>
        /// Note: remember to call MaxHealthCache.ResetCacheForPawn() when you are manually adjusting this value! We now no longer actively monitor the adjustment values!
        /// </summary>
        public float scaleAdjustment;

        public HediffCompProperties_MaxHPAdjust()
        {
            compClass = typeof(HediffComp_MaxHPAdjust);
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
        /// Returns the actual scaling factor given by the scale-adjustment;
        /// this value is to be directly multiplied to the max HP.
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

        public virtual string ProviderNamespaceString
        {
            get
            {
                // master class, added by EBF
                return "Elite Bionics Framework";
            }
        }

        public virtual bool IsPriority
        {
            get
            {
                // tihis is for PawnMorpher: we need to allow them to calculate things first
                return false;
            }
        }
    }
}
