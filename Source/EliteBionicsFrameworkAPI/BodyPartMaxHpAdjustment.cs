using System;
namespace EBF.API
{
    /// <summary>
    /// The struct that describes how the max HP of a body part should be adjusted.
    /// <para/>
    /// The rules here are the same as the XML-side rules; see the README for more details.
    /// </summary>
    public struct BodyPartMaxHpAdjustment
    {
        /// <summary>
        /// The linear adjustment to be applied; 0 indicates "no change" (i.e., +0).
        /// </summary>
        public int LinearAdjustment = 0;
        /// <summary>
        /// The scaling multiplier to be applied; 1 indicates "no scaling" (i.e., ×1).
        /// </summary>
        public float ScaleMultiplier = 1;
        /// <summary>
        /// The name of the provider of this effect.
        /// <para/>
        /// This is only for display purposes, so you may provide a "ModName".Translate() name here.
        /// </summary>
        public string EffectProvider = string.Empty;

        /// <summary>
        /// Constructs a new struct to describe how the body part max HP should be modified.
        /// </summary>
        public BodyPartMaxHpAdjustment()
        {
        }
    }
}
