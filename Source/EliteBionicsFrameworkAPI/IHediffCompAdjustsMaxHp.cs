namespace EBF.API
{
    /// <summary>
    /// An outward-facing interface for the HediffComp classes of external mods:
    /// they should delegate their body part max HP adjustment effects to this interface.
    /// <para/>
    /// At the moment, the effect should be constant since the final max HP will then be cached.
    /// <para/>
    /// This is the only approved place to notify EBF about C#-based body part max HP adjustment.
    /// </summary>
    public interface IHediffCompAdjustsMaxHp
    {
        /// <summary>
        /// The struct that describes how this HediffComp should adjust the max HP of the affected body part.
        /// <para/>
        /// See the README for some examples.
        /// </summary>
        public abstract BodyPartMaxHpAdjustment MaxHpAdjustment { get; }
    }
}
