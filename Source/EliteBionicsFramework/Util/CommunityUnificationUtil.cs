using EBF.Hediffs;
using EBF.Patches;
using HarmonyLib;
using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace EBF.Util
{
    public class CommunityUnificationUtil
    {
        private static bool hasCheckedPawnmorpherGetPartMaxHealth = false;
        private static MethodInfo PawnmorpherGetPartMaxHealth = null;
        private static Traverse Method_PawnmorpherGetPartMaxHealth = null;

        static CommunityUnificationUtil()
        {
            Method_PawnmorpherGetPartMaxHealth = Traverse.CreateWithType("Pawnmorph.BodyUtilities")?.Method("GetPartMaxHealth", new Type[2]
            {
                typeof(BodyPartRecord),
                typeof(Pawn),
            }, null);
        }

        public static String GetCompTipStringExtraDueToMaxHpAdjust(Pawn pawn, BodyPartDef def, HediffCompProperties_MaxHPAdjust props)
        {
            StringBuilder builder = new StringBuilder("");

            // Base HP tooltip
            float rawMaxHP = def.GetRawMaxHealth(pawn);
            builder.Append("Base HP: ");
            builder.Append(rawMaxHP);

            // Scale adjustment tooltip
            if (props.scaleAdjustment != 0)
            {
                builder.AppendLine();
                builder.Append("Max HP: ");
                builder.Append(props.ScaledAdjustmentDisplayString);
            }

            // Linear adjustment tooltip
            if (props.linearAdjustment != 0)
            {
                builder.AppendLine();
                builder.Append("Max HP: ");
                builder.Append(props.LinearAdjustmentDisplayString);
            }

            // Provider tooltip
            builder.AppendLine();
            builder.Append("Provided via: ");
            builder.Append(props.ProviderNamespaceString);

            return builder.ToString();
        }

        public static float GetPartMaxHealthFromPawnmorpher(BodyPartRecord record, Pawn p)
        {
            // we assert that Pawnmorpher is loaded; dont call without checking that Pawnmorpher exists
            return Reverse_Pawnmorpher_GetPartMaxHealth.GetPartMaxHealth(record, p);
            /*
            if (!hasCheckedPawnmorpherGetPartMaxHealth)
            {
                EliteBionicsFrameworkMain.LogError("1");
                PawnmorpherGetPartMaxHealth = AccessTools.Method(Type.GetType("Pawnmorph.BodyUtilities, Pawnmorph"), "GetPartMaxHealth");
                EliteBionicsFrameworkMain.LogError("2");
                EliteBionicsFrameworkMain.LogError(PawnmorpherGetPartMaxHealth?.ToString());
                hasCheckedPawnmorpherGetPartMaxHealth = true;
            }
            EliteBionicsFrameworkMain.LogError("3");
            if (PawnmorpherGetPartMaxHealth == null)
            {
                EliteBionicsFrameworkMain.LogError("Failed to reflect into Pawnmorpher->GetPartMaxHealth");
                return 0;
            }
            float testValue = (float) PawnmorpherGetPartMaxHealth.Invoke(null, new object[] { record, p });
            EliteBionicsFrameworkMain.LogError("" + testValue);
            return testValue;
            */
        }
    }
}
