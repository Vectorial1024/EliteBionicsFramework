using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBF.Hediffs;
using EBF.Patches;
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
            Prefix_BodyPart_GetMaxHealth.SuppressNextWarning();
            return def.GetMaxHealth(pawn);
        }

        /// <summary>
        /// Returns the actual max HP of this BodyPartRecord instance under the effects of EBF-enabled hediffs.
        /// </summary>
        /// <param name="record">The BodyPartRecord instance in question.</param>
        /// <param name="pawn">The pawn who is the owner of that BodyPartRecord instance.</param>
        /// <returns>The appropriate max HP of the BodyPartRecord under the effects of EBF-enabled hediffs</returns>
        public static float GetMaxHealthWithEBF(BodyPartRecord record, Pawn pawn)
        {
            float baseMaxHP = GetMaxHealthUnmodified(record.def, pawn);
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
                            // Negative values are denied.
                            totalScaledAdjustment *= (adjustorComp.scaleAdjustment + 1);
                        }
                    }
                }
            }

            float realMaxHP = baseMaxHP * totalScaledAdjustment + totalLinearAdjustment;
            return realMaxHP;
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
    }
}
