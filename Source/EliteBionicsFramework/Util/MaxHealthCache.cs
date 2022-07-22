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

        private static Dictionary<Pawn, Dictionary<BodyPartRecord, MaxHealthCacheRecord>> cache = new Dictionary<Pawn, Dictionary<BodyPartRecord, MaxHealthCacheRecord>>();

        public static float? GetCachedBodyPartMaxHealth(Pawn pawn, BodyPartRecord record)
        {
            Dictionary<BodyPartRecord, MaxHealthCacheRecord> innerDictionary;
            if (cache.TryGetValue(pawn, out innerDictionary))
            {
                // value exists
                MaxHealthCacheRecord cachedRecord;
                if (innerDictionary == null)
                {
                    return null;
                }
                if (innerDictionary.TryGetValue(record, out cachedRecord))
                {
                    // value exists
                    if (cachedRecord.expiresAtTick < Find.TickManager.TicksGame)
                    {
                        // expired
                        return null;
                    }
                    return cachedRecord.maxHealth;
                }
            }
            return null;
        }

        public static void SetCachedBodyPartMaxHealth(Pawn pawn, BodyPartRecord record, float maxHealth)
        {
            // a random number, to smooth out the spike of calling the methods
            int expiryTicks = Rand.RangeInclusive(60, 180);
            MaxHealthCacheRecord cachedRecord = new MaxHealthCacheRecord(maxHealth, Find.TickManager.TicksGame + expiryTicks);
            if (!cache.ContainsKey(pawn))
            {
                cache[pawn] = new Dictionary<BodyPartRecord, MaxHealthCacheRecord>();
            }
            cache[pawn][record] = cachedRecord;
        }

        public static void ResetCacheSpecifically(Pawn pawn, BodyPartRecord record)
        {
            // this is for when a hediff is added or removed, so that we can force a recalculation of values
            if (pawn == null)
            {
                // idk what you are talking about!
                return;
            }
            if (record == null)
            {
                // idk what you are talking about!
                return;
            }
            if (!cache.ContainsKey(pawn))
            {
                return;
            }
            cache[pawn].Remove(record);
        }

        public static void ResetCache()
        {
            // this is for when the game is loaded, so that we can force recalculation of everything
            cache.Clear();
        }
    }
}
