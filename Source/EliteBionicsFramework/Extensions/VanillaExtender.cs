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
            return EBFEndpoints.GetMaxHealthWithEBF(record, pawn);
        }

        /// <summary>
        /// Returns the finalized max HP of the given BodyPartRecord. This version utilizes a cache to reduce performance stress.
        /// <para/>
        /// This method is here to ensure maximized compatibility with other mods when I patch game codes.
        /// </summary>
        /// <param name="def">Extension method: the instance of BodyPartDef</param>
        /// <param name="pawn">Just as usual. Used to determine scaling bonus from pawn body size.</param>
        /// <param name="record">The body part that is requesting the MaxHealth. This field is required; without this, this method does not know what HP bonus to apply.</param>
        /// <returns></returns>
        public static float GetMaxHealth_Cached(this BodyPartDef def, Pawn pawn, BodyPartRecord record)
        {
            return EBFEndpoints.GetMaxHealthWithEBF(record, pawn, useCache: true);
        }

        /// <summary>
        /// Returns the max HP of the given BodyPartDef. It is basically a clone of the GetMaxHealth method.
        /// </summary>
        /// <param name="def"></param>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static float GetRawMaxHealth(this BodyPartDef def, Pawn pawn)
        {
            return EBFEndpoints.GetMaxHealthUnmodified(def, pawn);
        }

        /// <summary>
        /// Returns the max HP of the given BodyPartRecord with EBF effects considered.
        /// </summary>
        /// <param name="hediffSet"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static float GetPartMaxHealth(this HediffSet hediffSet, BodyPartRecord record)
        {
            return EBFEndpoints.GetBodyPartMaxHealthWithEBF(hediffSet, record);
        }

        /// <summary>
        /// Returns the max HP of this BodyPartRecord. You must also specify the Pawn.
        /// </summary>
        /// <param name="hediffSet"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static float GetMaxHealthForBodyPart(this BodyPartRecord record, Pawn pawn)
        {
            return EBFEndpoints.GetMaxHealthWithEBF(record, pawn);
        }

        /// <summary>
        /// Returns the max HP of the core body part of the pawn.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static float GetCorePartMaxHealth(this Pawn pawn)
        {
            BodyPartRecord corePart = pawn.def.race.body.corePart;
            return EBFEndpoints.GetMaxHealthWithEBF(corePart, pawn);
        }
    }
}
