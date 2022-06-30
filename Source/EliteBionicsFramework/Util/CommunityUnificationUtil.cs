using EBF.Hediffs;
using EBF.Patches;
using EBF.Patches.Unification;
using EBF.Patches.Unification.QualityBionics;
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

        private static Type CONN_Type_CompHealthIncrease = null;
        private static Type CONN_Type_CompPropsHealthIncrease = null;
        private static MethodInfo CONN_TryGetRelevantComp = null;

        private static Type CyberFauna_Type_CompPartHitPoints = null;
        private static Type CyberFauna_Type_CompPropsPartHitPoints = null;
        private static MethodInfo CyberFauna_TryGetRelevantComp = null;

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
            if (ModDetector.CONNIsLoaded)
            {
                CONN_Type_CompHealthIncrease = Type.GetType("CONN.HealthIncreaseComp, CONN");
                CONN_Type_CompPropsHealthIncrease = Type.GetType("CONN.CompProperties_HealthIncrease, CONN");
                CONN_TryGetRelevantComp = RW_Hediff_TryGetComp.MakeGenericMethod(new[] { CONN_Type_CompHealthIncrease });
            }
            if (ModDetector.CyberFaunaIsLoaded)
            {
                CyberFauna_Type_CompPartHitPoints = AccessTools.TypeByName("ProthesisHealth.HediffComp_PartHitPoints");
                CyberFauna_Type_CompPropsPartHitPoints = AccessTools.TypeByName("ProthesisHealth.HediffCompProperties_PartHitPoints");
                CyberFauna_TryGetRelevantComp = RW_Hediff_TryGetComp.MakeGenericMethod(new[] { CyberFauna_Type_CompPartHitPoints });
            }
        }

        public static string GetBodyPartSummaryTooltipStringDueToMaxHpAdjust(Pawn pawn, BodyPartRecord record)
        {
            List<HediffCompProperties_MaxHPAdjust> listHpProps = GetRealAndFakeHpPropsForUnification(pawn, record);
            if (listHpProps.Count == 0)
            {
                return null;
            }

            // summarize and print the stuff!
            int totalLinearAdjustment = 0;
            float totalScaledAdjustment = 1;
            foreach (HediffCompProperties_MaxHPAdjust props in listHpProps)
            {
                totalLinearAdjustment += props.linearAdjustment;
                if (props.scaleAdjustment + 1 > 0)
                {
                    // Only allow positive scaling values.
                    totalScaledAdjustment *= (props.scaleAdjustment + 1);
                }
            }

            // print
            StringBuilder builder = new StringBuilder("Body Part Max HP:");
            HediffCompProperties_MaxHPAdjust_Fake fakeComps = new HediffCompProperties_MaxHPAdjust_Fake()
            {
                linearAdjustment = totalLinearAdjustment,
                scaleAdjustment = totalScaledAdjustment - 1,
                providerNamespace = null,
            };
            int rawMaxHealth = (int) EBFEndpoints.GetMaxHealthUnmodified(record.def, pawn);
            builder.AppendLine();
            builder.Append(IndentationSpace);
            builder.Append("Base: ");
            builder.Append(rawMaxHealth);
            builder.Append(" HP");
            if (fakeComps.ScaledAdjustmentDisplayString.Length > 0)
            {
                builder.AppendLine();
                builder.Append(IndentationSpace);
                // products symbol
                builder.Append("\u220F ");
                builder.Append(fakeComps.ScaledAdjustmentDisplayString);
            }
            if (fakeComps.LinearAdjustmentDisplayString.Length > 0)
            {
                builder.AppendLine();
                builder.Append(IndentationSpace);
                // summation symbol
                builder.Append("\u2211 ");
                builder.AppendLine(fakeComps.LinearAdjustmentDisplayString);
            }

            return builder.ToString();
        }

        public static string GetCompTipStringExtraDueToMaxHpAdjust(Pawn pawn, BodyPartDef def, HediffCompProperties_MaxHPAdjust props)
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

            // Priority status
            if (props.IsPriority)
            {
                builder.AppendLine();
                builder.Append(IndentationSpace);
                builder.Append("The effects of this comp is prioritized before other comps.");
            }

            return builder.ToString();
        }

        public static string GetCompLabelInBracketsDueToMaxHpAdjust(Pawn pawn, HediffWithComps hediffWithComps)
        {
            if (EliteBionicsFrameworkMain.SettingHandle_DisplayHpDiffInHediffName.Value)
            {
                StringBuilder builder = new StringBuilder("HP: ");
                StringBuilder innerBuilder = new StringBuilder();
                BodyPartRecord record = hediffWithComps.Part;
                List<HediffCompProperties_MaxHPAdjust> listHpProps = GetRealAndFakeHpPropsForUnification(pawn, record, hediffWithComps);

                // summarize and print the stuff!
                int totalLinearAdjustment = 0;
                float totalScaledAdjustment = 1;
                foreach (HediffCompProperties_MaxHPAdjust props in listHpProps)
                {
                    totalLinearAdjustment += props.linearAdjustment;
                    if (props.scaleAdjustment + 1 > 0)
                    {
                        // Only allow positive scaling values.
                        totalScaledAdjustment *= (props.scaleAdjustment + 1);
                    }
                }

                // can print.
                HediffCompProperties_MaxHPAdjust_Fake fakeProps = new HediffCompProperties_MaxHPAdjust_Fake()
                {
                    linearAdjustment = totalLinearAdjustment,
                    scaleAdjustment = totalScaledAdjustment - 1,
                    providerNamespace = null,
                };
                if (fakeProps.scaleAdjustment != 0)
                {
                    innerBuilder.Append(fakeProps.ScaledAdjustmentDisplayString);
                }
                if (fakeProps.linearAdjustment != 0)
                {
                    if (innerBuilder.Length > 0)
                    {
                        innerBuilder.Append(", ");
                    }
                    innerBuilder.Append(fakeProps.LinearAdjustmentDisplayString);
                }

                if (innerBuilder.Length > 0)
                {
                    builder.Append(innerBuilder.ToString());
                    return builder.ToString();
                }
                // nothing to display
                return "";
            }
            else
            {
                return "";
            }
            return "";
        }

        public static string GetCompLabelInBracketsDueToMaxHpAdjust(HediffCompProperties_MaxHPAdjust props)
        {
            if (EliteBionicsFrameworkMain.SettingHandle_DisplayHpDiffInHediffName.Value)
            {
                StringBuilder builder = new StringBuilder("HP: ");
                StringBuilder innerBuilder = new StringBuilder();

                // we can print directly
                if (props.scaleAdjustment != 0)
                {
                    innerBuilder.Append(props.ScaledAdjustmentDisplayString);
                }
                if (props.linearAdjustment != 0)
                {
                    if (innerBuilder.Length > 0)
                    {
                        innerBuilder.Append(", ");
                    }
                    innerBuilder.Append(props.LinearAdjustmentDisplayString);
                }

                if (innerBuilder.Length > 0)
                {
                    builder.Append(innerBuilder.ToString());
                    return builder.ToString();
                }

                // nothing to display
                return "";
            }
            else
            {
                return "";
            }
            return "";
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

        public static List<HediffCompProperties_MaxHPAdjust> GetRealAndFakeHpPropsForUnification(Pawn pawn, BodyPartRecord record, HediffWithComps constraintComps = null)
        {
            List<HediffCompProperties_MaxHPAdjust> realAndFakeProps = new List<HediffCompProperties_MaxHPAdjust>();
            HediffSet hediffSet = pawn.health.hediffSet;

            foreach (HediffWithComps hediffWithComp in hediffSet.GetHediffs<HediffWithComps>())
            {
                if (hediffWithComp.Part != record)
                {
                    // wrong body part; skip
                    continue;
                }
                if (constraintComps != null && hediffWithComp != constraintComps)
                {
                    // wrong hediff; skip
                    continue;
                }
                foreach (HediffComp hediffComp in hediffWithComp.comps)
                {
                    if (hediffComp is HediffComp_MaxHPAdjust)
                    {
                        realAndFakeProps.Add((HediffCompProperties_MaxHPAdjust) hediffComp.props);
                        // read the next comp
                        continue;
                    }
                    // try if we can convert them into our own types
                    var hediffCompQualityBionics = TryConvertQualityBionicsCompToFakeHpComp(hediffComp);
                    if (hediffCompQualityBionics != null)
                    {
                        realAndFakeProps.Add(hediffCompQualityBionics);
                        continue;
                    }
                    var hediffCompConn = TryConvertConnCompToFakeHpComp(hediffComp);
                    if (hediffCompConn != null)
                    {
                        realAndFakeProps.Add(hediffCompConn);
                        continue;
                    }
                    var hediffCompCyberFauna = TryConvertCyberFaunaCompToFakeHpComp(hediffComp);
                    if (hediffCompCyberFauna != null)
                    {
                        realAndFakeProps.Add(hediffCompCyberFauna);
                        continue;
                    }
                }
            }

            return realAndFakeProps;
        }

        public static List<HediffCompProperties_MaxHPAdjust> GetRealAndFakeHpPropsForUnification(Hediff hediff)
        {
            List<HediffCompProperties_MaxHPAdjust> preppingList = new List<HediffCompProperties_MaxHPAdjust> { hediff.TryGetComp<HediffComp_MaxHPAdjust>()?.Props };
            preppingList.AddRange(GetFakeHpPropsForUnification(hediff));
            preppingList.RemoveAll(item => item == null);
            return preppingList;
        }

        public static List<HediffCompProperties_MaxHPAdjust> GetFakeHpPropsForUnification(Hediff hediff)
        {
            List<HediffCompProperties_MaxHPAdjust> list = new List<HediffCompProperties_MaxHPAdjust>();
            HediffWithComps hediffWithComps = hediff as HediffWithComps;
            if (hediffWithComps == null)
            {
                return list;
            }
            foreach (HediffComp comp in hediffWithComps.comps)
            {
                HediffCompProperties_MaxHPAdjust_Fake tempCheck = null;
                tempCheck = TryConvertQualityBionicsCompToFakeHpComp(comp);
                if (tempCheck != null)
                {
                    list.Add(tempCheck);
                    continue;
                }
                tempCheck = TryConvertConnCompToFakeHpComp(comp);
                if (tempCheck != null)
                {
                    list.Add(tempCheck);
                    continue;
                }
                tempCheck = TryConvertCyberFaunaCompToFakeHpComp(comp);
                if (tempCheck != null)
                {
                    list.Add(tempCheck);
                    continue;
                }
            }
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

        public static HediffCompProperties_MaxHPAdjust_Fake TryConvertConnCompToFakeHpComp(HediffComp comp)
        {
            if (CONN_Type_CompHealthIncrease == null)
            {
                // not loaded
                return null;
            }
            if (CONN_Type_CompHealthIncrease.IsInstanceOfType(comp))
            {
                // is instance of comp
                object temp = CONN_Type_CompHealthIncrease.GetProperty("Props").GetGetMethod().Invoke(comp, null);
                float deltaHp = (float)CONN_Type_CompPropsHealthIncrease.GetField("healthPointToAdd").GetValue(temp);
                HediffCompProperties_MaxHPAdjust_Fake fakeComp = new HediffCompProperties_MaxHPAdjust_Fake
                {
                    linearAdjustment = (int)deltaHp,
                    scaleAdjustment = 0,
                    providerNamespace = CONN_Type_CompHealthIncrease.Namespace
                };

                return fakeComp;
            }
            return null;
        }

        public static HediffCompProperties_MaxHPAdjust_Fake TryConvertCyberFaunaCompToFakeHpComp(HediffComp comp)
        {
            if (CyberFauna_Type_CompPartHitPoints == null)
            {
                // not loaded
                return null;
            }
            if (CyberFauna_Type_CompPartHitPoints.IsInstanceOfType(comp))
            {
                // is instance of comp
                object temp = CyberFauna_Type_CompPartHitPoints.GetProperty("Props").GetGetMethod().Invoke(comp, null);
                float multiplier = (float)CyberFauna_Type_CompPropsPartHitPoints.GetField("multiplier").GetValue(temp);
                HediffCompProperties_MaxHPAdjust_Fake fakeComp = new HediffCompProperties_MaxHPAdjust_Fake
                {
                    linearAdjustment = 0,
                    scaleAdjustment = multiplier - 1,
                    providerNamespace = CyberFauna_Type_CompPartHitPoints.Namespace
                };

                return fakeComp;
            }
            return null;
        }
    }
}
