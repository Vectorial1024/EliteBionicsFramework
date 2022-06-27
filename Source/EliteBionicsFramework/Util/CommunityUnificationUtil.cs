using EBF.Hediffs;
using EBF.Patches;
using EBF.Patches.Unification;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace EBF.Util
{
    [StaticConstructorOnStartup]
    public class CommunityUnificationUtil
    {
        private static bool hasCheckedPawnmorpherGetPartMaxHealth = false;
        private static MethodInfo PawnmorpherGetPartMaxHealth = null;
        private static Traverse Method_PawnmorpherGetPartMaxHealth = null;

        private static MethodInfo RW_Hediff_TryGetComp = null;

        private static Type QualityBionics_Type_Main = null;
        private static FieldInfo QualityBionics_Field_MainSettings = null;
        private static Type QualityBionics_Type_QualityBionicsSettings = null;
        private static MethodInfo QualityBionics_Method_GetQualityMultiplier = null;
        private static Type QualityBionics_Type_CompQualityBionics = null;
        private static MethodInfo QualityBionics_TryGetRelevantComp = null;

        // hmmm... would we allow for others to modify the indentation strength?
        private static string IndentationSpace = "    ";

        static CommunityUnificationUtil()
        {
            Method_PawnmorpherGetPartMaxHealth = Traverse.CreateWithType("Pawnmorph.BodyUtilities")?.Method("GetPartMaxHealth", new Type[2]
            {
                typeof(BodyPartRecord),
                typeof(Pawn),
            }, null);

            RW_Hediff_TryGetComp = typeof(HediffUtility).GetMethod(nameof(HediffUtility.TryGetComp));
            if (ModDetector.QualityBionicsIsLoaded)
            {
                QualityBionics_Type_Main = Type.GetType("QualityBionics.QualityBionicsMod, QualityBionics");
                QualityBionics_Field_MainSettings = QualityBionics_Type_Main.GetField("settings");

                QualityBionics_Type_QualityBionicsSettings = Type.GetType("QualityBionics.QualityBionicsSettings, QualityBionics");
                QualityBionics_Method_GetQualityMultiplier = QualityBionics_Type_QualityBionicsSettings.GetMethod("GetQualityMultipliersForHP");

                QualityBionics_Type_CompQualityBionics = Type.GetType("QualityBionics.HediffCompQualityBionics, QualityBionics");
                QualityBionics_TryGetRelevantComp = RW_Hediff_TryGetComp.MakeGenericMethod(new[] { QualityBionics_Type_CompQualityBionics });
            }
        }

        public static String GetCompTipStringExtraDueToMaxHpAdjust(Pawn pawn, BodyPartDef def, HediffCompProperties_MaxHPAdjust props)
        {
            StringBuilder builder = new StringBuilder("");

            // Provider first: we want to tidy uu the display and avoid UI confusion
            builder.Append("From: ");
            builder.Append(props.ProviderNamespaceString);

            // Base HP tooltip
            float rawMaxHP = def.GetRawMaxHealth(pawn);
            builder.AppendLine();
            builder.Append(IndentationSpace);
            builder.Append("Base HP: ");
            builder.Append(rawMaxHP);

            // Scale adjustment tooltip
            if (props.scaleAdjustment != 0)
            {
                builder.AppendLine();
                builder.Append(IndentationSpace);
                builder.Append("Max HP: ");
                builder.Append(props.ScaledAdjustmentDisplayString);
            }

            // Linear adjustment tooltip
            if (props.linearAdjustment != 0)
            {
                builder.AppendLine();
                builder.Append(IndentationSpace);
                builder.Append("Max HP: ");
                builder.Append(props.LinearAdjustmentDisplayString);
            }

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

        public static List<HediffCompProperties_MaxHPAdjust> GetFakeHpPropsForUnification(Hediff hediff)
        {
            List<HediffCompProperties_MaxHPAdjust> list = new List<HediffCompProperties_MaxHPAdjust>();
            HediffCompProperties_MaxHPAdjust_Fake temp = TryMakeFakePropsOfQualityBionics(hediff);
            if (temp != null)
            {
                list.Add(temp);
            }
            // TryAddFakePropsOfQualityBionics(hediff, list);
            return list;
        }

        public static HediffCompProperties_MaxHPAdjust_Fake TryConvertQualityBionicsCompToFakeHpComp(HediffComp comp)
        {
            if (QualityBionics_Type_Main == null)
            {
                // not loaded
                return null;
            }
            if (QualityBionics_Type_CompQualityBionics.IsInstanceOfType(comp))
            {
                // is instance of comp
                QualityCategory quality = (QualityCategory)QualityBionics_Type_CompQualityBionics.GetField("quality").GetValue(comp);
                // EliteBionicsFrameworkMain.LogError("quality " + quality.ToStringSafe());
                object qualityBionicsSettings = QualityBionics_Type_Main.GetField("settings").GetValue(null);
                // EliteBionicsFrameworkMain.LogError("settings " + qualityBionicsSettings.ToStringSafe());
                float scalingMultiplier = Reverse_QualityBionics_GetQualityMultiplier.GetQualityMultipliersForHP(qualityBionicsSettings, quality);
                // float scalingMultiplier = (float) QualityBionics_Method_GetQualityMultiplier.Invoke(qualityBionicsSettings, new object[] { quality });
                // EliteBionicsFrameworkMain.LogError("scaler " + scalingMultiplier.ToStringSafe());
                HediffCompProperties_MaxHPAdjust_Fake fakeComp = new HediffCompProperties_MaxHPAdjust_Fake
                {
                    linearAdjustment = 0,
                    scaleAdjustment = scalingMultiplier - 1,
                    providerNamespace = QualityBionics_Type_Main.Namespace
                };
                // EliteBionicsFrameworkMain.LogError("fakeComp " + fakeComp.ToStringSafe());
                return fakeComp;
            }
            return null;
        }

        public static HediffCompProperties_MaxHPAdjust_Fake TryMakeFakePropsOfQualityBionics(Hediff hediff)
        {
            if (QualityBionics_Type_Main == null)
            {
                // not loaded
                return null;
            }
            // todo why does this return null?
            object hediffCompQualityBionics = QualityBionics_TryGetRelevantComp.Invoke(null, new object[] { hediff });
            // EliteBionicsFrameworkMain.LogError("hediff " + hediffCompQualityBionics.ToStringSafe());
            if (hediffCompQualityBionics != null)
            {
                QualityCategory quality = (QualityCategory)QualityBionics_Type_CompQualityBionics.GetField("quality").GetValue(hediffCompQualityBionics);
                // EliteBionicsFrameworkMain.LogError("quality " + quality.ToStringSafe());
                object qualityBionicsSettings = QualityBionics_Type_Main.GetField("settings").GetValue(null);
                // EliteBionicsFrameworkMain.LogError("settings " + qualityBionicsSettings.ToStringSafe());
                float scalingMultiplier = Reverse_QualityBionics_GetQualityMultiplier.GetQualityMultipliersForHP(qualityBionicsSettings, quality);
                // float scalingMultiplier = (float) QualityBionics_Method_GetQualityMultiplier.Invoke(qualityBionicsSettings, new object[] { quality });
                // EliteBionicsFrameworkMain.LogError("scaler " + scalingMultiplier.ToStringSafe());
                HediffCompProperties_MaxHPAdjust_Fake fakeComp = new HediffCompProperties_MaxHPAdjust_Fake
                {
                    linearAdjustment = 0,
                    scaleAdjustment = scalingMultiplier - 1,
                    providerNamespace = QualityBionics_Type_Main.Namespace
                };
                // EliteBionicsFrameworkMain.LogError("fakeComp " + fakeComp.ToStringSafe());
                return fakeComp;
            }
            return null;
        }

        private static void TryAddFakePropsOfQualityBionics(Hediff hediff, List<HediffCompProperties_MaxHPAdjust> list)
        {
            return;
            if (QualityBionics_Type_Main == null)
            {
                // not loaded
                return;
            }
            object hediffCompQualityBionics = QualityBionics_TryGetRelevantComp.Invoke(null, new object[] { hediff });
            EliteBionicsFrameworkMain.LogError("hediff " + hediffCompQualityBionics?.ToStringSafe());
            if (hediffCompQualityBionics != null)
            {
                QualityCategory quality = (QualityCategory) QualityBionics_Type_CompQualityBionics.GetField("quality").GetValue(hediffCompQualityBionics);
                EliteBionicsFrameworkMain.LogError("quality " + quality.ToStringSafe());
                object qualityBionicsSettings = QualityBionics_Type_Main.GetField("settings").GetValue(null);
                EliteBionicsFrameworkMain.LogError("settings " + qualityBionicsSettings.ToStringSafe());
                float scalingMultiplier = Reverse_QualityBionics_GetQualityMultiplier.GetQualityMultipliersForHP(qualityBionicsSettings, quality);
                //float scalingMultiplier = (float)QualityBionics_Method_GetQualityMultiplier.Invoke(qualityBionicsSettings, new object[] { quality });
                HediffCompProperties_MaxHPAdjust_Fake fakeComp = new HediffCompProperties_MaxHPAdjust_Fake
                {
                    linearAdjustment = 0,
                    scaleAdjustment = scalingMultiplier - 1
                };
                list.Add(fakeComp);
            }
        }
    }
}
