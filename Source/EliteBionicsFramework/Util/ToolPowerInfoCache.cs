using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EBF.Util
{
    /// <summary>
    /// A class for handling and caching the tool power adjustment information for repeated usage.
    /// </summary>
    public class ToolPowerInfoCache
    {
        private readonly struct ToolPowerInfoRecord(int expiresAtTick, ToolPowerAdjustInfo adjustInfo)
        {
            public readonly int expiresAtTick = expiresAtTick;
            public readonly ToolPowerAdjustInfo adjustInfo = adjustInfo;
        }

        /// <summary>
        /// The dictionary of Pawn -> AttackInfo -> AdjustInfo/null
        /// </summary>
        private static readonly Dictionary<Pawn, Dictionary<VerbAttackInfo, ToolPowerInfoRecord>> cache = [];

        public static ToolPowerAdjustInfo? GetCachedToolPowerInfo(Pawn pawn, VerbAttackInfo attackInfo)
        {
            if (pawn == null || !attackInfo.IsValid)
            {
                // these are required parameters!
                return null;
            }
            if (cache.TryGetValue(pawn, out Dictionary<VerbAttackInfo, ToolPowerInfoRecord> innerDict))
            {
                // value exists
                if (innerDict == null)
                {
                    return null;
                }
                if (innerDict.TryGetValue(attackInfo, out ToolPowerInfoRecord cachedRecord))
                {
                    // value exists
                    if (cachedRecord.expiresAtTick < Find.TickManager.TicksGame)
                    {
                        // expired
                        return null;
                    }
                    return cachedRecord.adjustInfo;
                }
            }
            return null;
        }

        public static void SetCachedToolPowerInfo(Pawn pawn, VerbAttackInfo attackInfo, ToolPowerAdjustInfo adjustInfo)
        {
            // a random number, to smooth out the spike of calling the methods
            // note: we are now using the update-on-delta paradigm, so the recalculation interval is now very long
            // 60 ticks in 1 IRL second
            // this means a guaranteed update in Rand(16.7 minutes, 50 minutes), which is practically very long
            // also note: we push/pop rand states to implement multiplayer compatibility
            Rand.PushState();
            int expiryTicks = Rand.RangeInclusive(60000, 180000);
            Rand.PopState();
            ToolPowerInfoRecord cachedRecord = new(Find.TickManager.TicksGame + expiryTicks, adjustInfo);
            lock (cache)
            {
                if (!cache.ContainsKey(pawn))
                {
                    cache[pawn] = [];
                }
                cache[pawn][attackInfo] = cachedRecord;
            }
        }

        public static void ResetCacheForPawn(Pawn pawn)
        {
            // this is for when the tool power is changed due to eg life-stage is changed.
            // this is applied to the entire pawn, so we can just forget everything and let the framework recalculate the values on-demand
            if (pawn == null)
            {
                // idk what you are talking about!
                return;
            }
            // Log.Error("Resestting cache: " + pawn.ToStringSafe() + "\n" + Environment.StackTrace);
            lock (cache)
            {
                if (!cache.ContainsKey(pawn))
                {
                    return;
                }
                cache.Remove(pawn);
            }
        }

        public static void ResetCache()
        {
            // this is for when the game is loaded, so that we can force recalculation of everything
            lock (cache)
            {
                cache.Clear();
            }
        }
    }
}
