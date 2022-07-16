using System.Collections.Generic;
using Verse;

namespace EBF.Util
{

    public class MaxHealthCache
    {
        private struct MaxHealthCacheRecord
        {
            public float maxHealth;
            public int expiresAtTick;

            public MaxHealthCacheRecord(float maxHealth, int expiresAtTick)
            {
                this.maxHealth = maxHealth;
                this.expiresAtTick = expiresAtTick;
            }
        }

        private static Dictionary<BodyPartRecord, MaxHealthCacheRecord> cache = new Dictionary<BodyPartRecord, MaxHealthCacheRecord>();

        public static float? GetCachedBodyPartMaxHealth(BodyPartRecord record)
        {
            MaxHealthCacheRecord cachedRecord;
            if (cache.TryGetValue(record, out cachedRecord))
            {
                // value exists
                if (cachedRecord.expiresAtTick < Find.TickManager.TicksGame)
                {
                    return null;
                }
                return cachedRecord.maxHealth;
            }
            return null;
        }

        public static void SetCachedBodyPartMaxHealth(BodyPartRecord record, float maxHealth)
        {
            // a random number, to smooth out the spike of calling the methods
            int expiryTicks = Rand.RangeInclusive(60, 180);
            MaxHealthCacheRecord cachedRecord = new MaxHealthCacheRecord(maxHealth, Find.TickManager.TicksGame + expiryTicks);
            cache[record] = cachedRecord;
        }

        public static void ResetCacheSpecifically(BodyPartRecord record)
        {
            // this is for when a hediff is added or removed, so that we can force a recalculation of values
            if (record == null)
            {
                // idk what you are talking about!
                return;
            }
            cache.Remove(record);
        }

        public static void ResetCache()
        {
            // this is for when the game is loaded, so that we can force recalculation of everything
            cache.Clear();
        }
    }
}
