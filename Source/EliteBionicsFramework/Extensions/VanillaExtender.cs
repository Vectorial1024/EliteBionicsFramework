using EBF.Hediffs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EBF
{
    public static class VanillaExtender
    {
        /// <summary>
        /// Returns the finalized max HP of the given BodyPartRecord.
        /// <para/>
        /// This method is here to ensure maximized compatibility with other mods when I patch game codes.
        /// </summary>
        /// <param name="def">Extension method: the instance of BodyPartDef</param>
        /// <param name="pawn">Just as usual. Used to determine scaling bonus from pawn body size.</param>
        /// <param name="record">The body part that is requesting the MaxHealth. This field is required; without this, this method does not know what HP bonus to apply.</param>
        /// <returns></returns>
        public static float GetMaxHealth(this BodyPartDef def, Pawn pawn, BodyPartRecord record)
        {
            // Log.Message("Who dareth disturb my slumber???");
            // return def.GetRawMaxHealth(pawn);
            return record.GetMaxHealthForBodyPart(pawn);
        }

        /// <summary>
        /// Returns the max HP of the given BodyPartDef. It is basically a clone of the GetMaxHealth method.
        /// </summary>
        /// <param name="def"></param>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static float GetRawMaxHealth(this BodyPartDef def, Pawn pawn)
        {
            return Mathf.CeilToInt(def.hitPoints * pawn.HealthScale);
        }

        /// <summary>
        /// Returns the max HP of the given BodyPartRecord.
        /// </summary>
        /// <param name="hediffSet"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static float GetPartMaxHealth(this HediffSet hediffSet, BodyPartRecord record)
        {
            float linearAdjustment = 0;
            foreach (Hediff hediff in hediffSet.hediffs)
            {
                if (hediff.Part == record)
                {
                    HediffCompProperties_MaxHPAdjust adjustorComp = hediff.def.CompProps<HediffCompProperties_MaxHPAdjust>();
                    if (adjustorComp != null)
                    {
                        linearAdjustment += adjustorComp.linearAdjustment;
                    }
                }
            }

            // We can't allow max health to drop to 0, that would be problematic.
            return Mathf.Max(record.def.GetRawMaxHealth(hediffSet.pawn) + linearAdjustment, 0);
        }

        /// <summary>
        /// Returns the max HP of this BodyPartRecord. You must also specify the Pawn.
        /// </summary>
        /// <param name="hediffSet"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static float GetMaxHealthForBodyPart(this BodyPartRecord record, Pawn pawn)
        {
            return pawn.health.hediffSet.GetPartMaxHealth(record);
        }

        /// <summary>
        /// Returns the max HP of the core body part of the pawn.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static float GetCorePartMaxHealth(this Pawn pawn)
        {
            return pawn.health.hediffSet.GetPartMaxHealth(pawn.def.race.body.corePart);
        }
    }
}
