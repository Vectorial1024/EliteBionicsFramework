using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBF.Hediffs;
using EBF.Patches;
using EBF.Util;
using UnityEngine;
using Verse;

namespace EBF
{
    /// <summary>
    /// An outward-facing class for other parts of this mod and other external mods to call
    /// to obtain the true max HP of body parts affected by EBF-enabled Hediffs.
    /// <para/>
    /// This is the only trustable source of EBF-affected body part max HP.
    /// </summary>
    public class EBFEndpoints
    {
        #region MaxHP

        /// <summary>
        /// Returns the unmodified, raw max HP of this body part def. This returns def.GetMaxHealth(pawn); .
        /// <para/>
        /// The only difference compared with directly calling vanilla code is that, by using this method,
        /// the use case between general inquiry and specific inquiry of body part max HP is clarified,
        /// and no "clarification requierd" warning message will be displayed in the log.
        /// </summary>
        /// <param name="def">The BodyPartDef isntance obtained from a BodyPartRecord isntance.</param>
        /// <param name="pawn">The pawn who is the owner of that BodyPartRecord instance.</param>
        /// <returns>def.GetMaxHealth(pawn)</returns>
        public static float GetMaxHealthUnmodified(BodyPartDef def, Pawn pawn)
        {
            PostFix_BodyPart_GetMaxHealth.SuppressNextWarning();
            return def.GetMaxHealth(pawn);
        }

        /// <summary>
        /// Returns the actual max HP of this BodyPartRecord instance under the effects of EBF-enabled hediffs.
        /// <para/>
        /// Note: since EBF v5.1.3, the HP value returned is the cached value by default. To force EBF to calculate a fresh value, set useCache = false.
        /// </summary>
        /// <param name="record">The BodyPartRecord instance in question.</param>
        /// <param name="pawn">The pawn who is the owner of that BodyPartRecord instance.</param>
        /// <param name="useCache">Whether to use the max-health cache, for cases where max-health is constantly queried but unlikely to change.</param>
        /// <returns>The appropriate max HP of the BodyPartRecord under the effects of EBF-enabled hediffs</returns>
        public static float GetMaxHealthWithEBF(BodyPartRecord record, Pawn pawn, bool useCache = true)
        {
            if (useCache)
            {
                float? cachedValue = MaxHealthCache.GetCachedBodyPartMaxHealth(pawn, record);
                if (cachedValue != null)
                {
                    return cachedValue.Value;
                }
            }
            float baseMaxHP;
            if (ModDetector.PawnmorpherIsLoadedCached)
            {
                baseMaxHP = CommunityUnificationUtil.GetPartMaxHealthFromPawnmorpher(record, pawn);
            }
            else
            {
                baseMaxHP = GetMaxHealthUnmodified(record.def, pawn);
            }
            HediffSet hediffSet = pawn.health.hediffSet;
            float totalLinearAdjustment = 0;
            float totalScaledAdjustment = 1;

            foreach (Hediff hediff in hediffSet.hediffs)
            {
                if (hediff.Part == record)
                {
                    HediffCompProperties_MaxHPAdjust adjustorComp = hediff.def.CompProps<HediffCompProperties_MaxHPAdjust>();
                    if (adjustorComp != null)
                    {
                        totalLinearAdjustment += adjustorComp.linearAdjustment;
                        if (adjustorComp.scaleAdjustment + 1 > 0)
                        {
                            // Only allow positive scaling values.
                            totalScaledAdjustment *= (adjustorComp.scaleAdjustment + 1);
                        }
                    }
                    List<HediffCompProperties_MaxHPAdjust> propsList = CommunityUnificationUtil.GetFakeHpPropsForUnification(hediff);
                    foreach (HediffCompProperties_MaxHPAdjust fakeProps in propsList)
                    {
                        if (fakeProps.IsPriority)
                        {
                            // priority currently is expected for pawnmorpher only, and we have already handled it above.
                            continue;
                        }
                        totalLinearAdjustment += fakeProps.linearAdjustment;
                        if (fakeProps.scaleAdjustment + 1 > 0)
                        {
                            // Only allow positive scaling values.
                            totalScaledAdjustment *= (fakeProps.scaleAdjustment + 1);
                        }
                    }
                }
            }

            float realMaxHP = Mathf.RoundToInt(baseMaxHP * totalScaledAdjustment) + totalLinearAdjustment;
            // checked in RimWorld 1.5; also does CeilToInt because that is what the vanilla game is doing.
            realMaxHP = Mathf.CeilToInt(realMaxHP);
            // must be at least 1
            float calculatedValue = Mathf.Max(realMaxHP, 1);
            if (useCache)
            {
                MaxHealthCache.SetCachedBodyPartMaxHealth(pawn, record, calculatedValue);
            }
            return calculatedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hediffSet"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static float GetBodyPartMaxHealthUnmodified(HediffSet hediffSet, BodyPartRecord record)
        {
            return GetMaxHealthUnmodified(record.def, hediffSet.pawn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hediffSet">The HediffSet instance.</param>
        /// <param name="record">The BodyPartRecord instance.</param>
        /// <returns></returns>
        public static float GetBodyPartMaxHealthWithEBF(HediffSet hediffSet, BodyPartRecord record)
        {
            return GetMaxHealthWithEBF(record, hediffSet.pawn);
        }

        #endregion

        #region ToolPower

        /// <summary>
        /// Returns the tool/verb power adjustment to be applied on the given (melee) attack by the pawn with the given body parts, etc under the effects of EBF-enabled hediffs.
        /// </summary>
        /// <param name="pawn">The pawn who is attacking</param>
        /// <param name="tool">The tool which the pawn is using to attack</param>
        /// <param name="source">The hediff source, if exists; this is used to identify between e.g. the left hand and the right hand, where both have the same tool "hand".</param>
        /// <param name="useCache">Whether to use the max-health cache, for cases where max-health is constantly queried but unlikely to change.</param>
        /// <returns>The appropriate (melee) attack power adjustment under the effects of EBF-enabled hediffs.</returns>
        public static ToolPowerAdjustInfo? GetToolPowerAdjustInfoWithEbf(Pawn pawn, Tool tool, HediffComp_VerbGiver source, bool useCache = true)
        {
            VerbAttackInfo attackInfo = new(tool, source);
            if (pawn == null || !attackInfo.IsValid)
            {
                // invalid; we should not give you any info
                return null;
            }

            // basic info are valid; do it.
            if (useCache)
            {
                ToolPowerAdjustInfo? cachedValue = ToolPowerInfoCache.GetCachedToolPowerInfo(pawn, attackInfo);
                if (cachedValue != null)
                {
                    return cachedValue.Value;
                }
            }
            // value does not exist; calculate it!
            /*
             * after code review, we are basically doing the following steps:
             * - determine where to look for potential EBF hediffs in the pawn bodypart-hediff tree (this depends on the exact format of the VerbAttackInfo)
             * - extract those hediffs
             * - aggregate the effects
             * - apply changes to ingame numbers
             */
            // wip
            throw new NotImplementedException();
        }

        #endregion
    }
}
