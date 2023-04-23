using System;
using System.Collections.Concurrent;
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

        private static ConcurrentDictionary<Pawn, ConcurrentDictionary<BodyPartRecord, MaxHealthCacheRecord>> cache = new ConcurrentDictionary<Pawn, ConcurrentDictionary<BodyPartRecord, MaxHealthCacheRecord>>();

        public static float? GetCachedBodyPartMaxHealth(Pawn pawn, BodyPartRecord record)
        {
            ConcurrentDictionary<BodyPartRecord, MaxHealthCacheRecord> innerDictionary;
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
            // note: we are now using the update-on-delta paradigm, so the recalculation interval is now very long
            // 60 ticks in 1 IRL second
            // this means a guaranteed update in Rand(16.7 minutes, 50 minutes), which is practically very long
            // also note: we push/pop rand states to implement multiplayer compatibility
            Rand.PushState();
            int expiryTicks = Rand.RangeInclusive(60000, 180000);
            Rand.PopState();
            MaxHealthCacheRecord cachedRecord = new MaxHealthCacheRecord(maxHealth, Find.TickManager.TicksGame + expiryTicks);
            if (!cache.ContainsKey(pawn))
            {
                cache[pawn] = new ConcurrentDictionary<BodyPartRecord, MaxHealthCacheRecord>();
            }
            cache[pawn][record] = cachedRecord;
        }

        public static void ResetCacheForPawn(Pawn pawn)
        {
            // this is for when the max HP value is changed due to eg life-stage is changed.
            // this is applied to the entire pawn, so we can just forget everything and let the framework recalculate the values on-demand
            if (pawn == null)
            {
                // idk what you are talking about!
                return;
            }
            if (!cache.ContainsKey(pawn))
            {
                return;
            }
            Log.Error("Resetting health cache for pawn " + pawn.ToStringSafe() + "; stack trace in details\n" + Environment.StackTrace);
            cache.TryRemove(pawn, out _);
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
            cache[pawn].TryRemove(record, out _);
        }

        public static void ResetCache()
        {
            // this is for when the game is loaded, so that we can force recalculation of everything
            cache.Clear();
        }
    }
}
