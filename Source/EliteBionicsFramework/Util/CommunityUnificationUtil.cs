using EBF.Hediffs;
using EBF.Patches;
using EBF.Patches.Unification;
using EBF.Patches.Unification.Pawnmorpher;
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
        private static MethodInfo RW_Hediff_TryGetComp = null;

        private static Type QualityBionics_Type_Main = null;
        private static FieldInfo QualityBionics_Field_MainSettings = null;
        private static Type QualityBionics_Type_QualityBionicsSettings = null;
        private static MethodInfo QualityBionics_Method_GetQualityMultiplier = null;
        private static Type QualityBionics_Type_CompQualityBionics = null;
        private static MethodInfo QualityBionics_TryGetRelevantComp = null;

        // note: due to CONN officially changing to use EBF directly, there is no longer any need to keep CONN-related fields

        private static Type CyberFauna_Type_CompPartHitPoints = null;
        private static Type CyberFauna_Type_CompPropsPartHitPoints = null;
        private static MethodInfo CyberFauna_TryGetRelevantComp = null;

        private static Type Pawnmorpher_Type_MutationUtilities = null;
        private static Type Pawnmorpher_Type_MutationTracker = null;
        private static Type Pawnmorpher_Type_HediffAddedMutation = null;
        private static Type Pawnmorpher_Type_MutationStage = null;

        // hmmm... would we allow for others to modify the indentation strength?
        private static string IndentationSpace = "    ";

        // special handling for ProthesisHealth which powers CyberFauna and MechalitCore
        private static bool HasLoadedProthesisHealth = false;

        static CommunityUnificationUtil()
        {
            var methodSignature = new Type[] { typeof(Hediff) };
            RW_Hediff_TryGetComp = typeof(HediffUtility).GetMethod(nameof(HediffUtility.TryGetComp), methodSignature);
            methodSignature = null;

            // we are splitting this into several functions so that debugging can give us meaningful stacktraces
            TryPatchQualityBionics();
            TryPatchCyberFauna();
            TryPatchMechalitCore();
            TryPatchPawnmorpher();
        }

        #region Community Unification Mod Patching

        private static void TryPatchQualityBionics()
        {
            if (ModDetector.QualityBionicsIsLoaded)
            {
                try
                {
                    QualityBionics_Type_Main = Type.GetType("QualityBionics.QualityBionicsMod, QualityBionics");
                    QualityBionics_Field_MainSettings = QualityBionics_Type_Main.GetField("settings");

                    QualityBionics_Type_QualityBionicsSettings = Type.GetType("QualityBionics.QualityBionicsSettings, QualityBionics");
                    QualityBionics_Method_GetQualityMultiplier = QualityBionics_Type_QualityBionicsSettings.GetMethod("GetQualityMultipliersForHP");

                    QualityBionics_Type_CompQualityBionics = Type.GetType("QualityBionics.HediffCompQualityBionics, QualityBionics");
                    QualityBionics_TryGetRelevantComp = RW_Hediff_TryGetComp.MakeGenericMethod(new[] { QualityBionics_Type_CompQualityBionics });
                }
                catch (ArgumentNullException)
                {
                    // we failed to make a generic method
                    EliteBionicsFrameworkMain.LogError("Something about Quality Bionics changed; please report this to us.");
                }
            }
        }

        private static void TryPatchCyberFauna()
        {

            if (ModDetector.CyberFaunaIsLoaded)
            {
                // test
                if (!ModDetector.CyberFaunaOfficialIsLoaded)
                {
                    throw new NotSupportedException(EliteBionicsFrameworkMain.MODPREFIX + "We do not support your version of Cyber Fauna.");
                }
                try
                {
                    CyberFauna_Type_CompPartHitPoints = AccessTools.TypeByName("ProthesisHealth.HediffComp_PartHitPoints");
                    CyberFauna_Type_CompPropsPartHitPoints = AccessTools.TypeByName("ProthesisHealth.HediffCompProperties_PartHitPoints");
                    CyberFauna_TryGetRelevantComp = RW_Hediff_TryGetComp.MakeGenericMethod(new[] { CyberFauna_Type_CompPartHitPoints });
                    HasLoadedProthesisHealth = true;
                }
                catch (ArgumentNullException)
                {
                    // we failed to make a generic method
                    EliteBionicsFrameworkMain.LogError("Something about CyberFauna changed; please report this to us.");
                }
            }
        }

        private static void TryPatchMechalitCore()
        {

            if (ModDetector.MechalitCoreIsLoaded)
            {
                // it is infuriating that both cyber fauna and mechalit core is using the same dll for stuff yet there are two copies of it in total
                if (!ModDetector.MechalitCoreOfficialIsLoaded)
                {
                    throw new NotSupportedException(EliteBionicsFrameworkMain.MODPREFIX + "We do not support your version of Mechalit Core.");
                }
                try
                {
                    if (!HasLoadedProthesisHealth)
                    {
                        CyberFauna_Type_CompPartHitPoints = AccessTools.TypeByName("ProthesisHealth.HediffComp_PartHitPoints");
                        CyberFauna_Type_CompPropsPartHitPoints = AccessTools.TypeByName("ProthesisHealth.HediffCompProperties_PartHitPoints");
                        CyberFauna_TryGetRelevantComp = RW_Hediff_TryGetComp.MakeGenericMethod(new[] { CyberFauna_Type_CompPartHitPoints });
                        HasLoadedProthesisHealth = true;
                    }
                }
                catch (ArgumentNullException)
                {
                    // we failed to make a generic method
                    EliteBionicsFrameworkMain.LogError("Something about MechalitCore changed; please report this to us.");
                }
            }
        }

        private static void TryPatchPawnmorpher()
        {
            if (ModDetector.PawnmorpherIsLoaded)
            {
                try
                {
                    Pawnmorpher_Type_MutationUtilities = AccessTools.TypeByName("Pawnmorph.MutationUtilities");
                    Pawnmorpher_Type_MutationTracker = AccessTools.TypeByName("Pawnmorph.MutationTracker");
                    Pawnmorpher_Type_HediffAddedMutation = AccessTools.TypeByName("Pawnmorph.Hediff_AddedMutation");
                    Pawnmorpher_Type_MutationStage = AccessTools.TypeByName("Pawnmorph.Hediffs.MutationStage");
                }
                catch (ArgumentNullException)
                {
                    // we failed to make a generic method
                    EliteBionicsFrameworkMain.LogError("Something about Pawnmorpher changed; please report this to us.");
                }
            }
        }

        #endregion

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
            builder.Append(rawMaxHealth.ToStringCached());
            builder.Append(" HP");
            if (pawn.HealthScale != Reverse_Pawn_HealthScale.GetOriginalHealthScale(pawn))
            {
                // we got some other mods modifying the health scale
                int healthScaleMultiplerPercentage = (int) (pawn.HealthScale / Reverse_Pawn_HealthScale.GetOriginalHealthScale(pawn) * 100);
                builder.AppendLine();
                builder.Append(IndentationSpace);
                // products symbol
                builder.Append("Extra health scale: ×");
                builder.Append(healthScaleMultiplerPercentage.ToStringCached());
                builder.Append("%");
            }
            // pawnmorpher stuff
            HediffCompProperties_MaxHPAdjust_Fake pawnmorpherComps = TryExtractPawnmorpherHediffToFakeHpComp(pawn, record);
            if (pawnmorpherComps != null)
            {
                builder.AppendLine();
                builder.Append(IndentationSpace);
                builder.Append("Pawn-Morphing: ");
                builder.Append(pawnmorpherComps.ScaledAdjustmentDisplayString);
            }
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
            if (!EliteBionicsFrameworkMain.SettingHandle_DisplayHpDiffInHediffName)
            {
                return "";
            }

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

        public static string GetCompLabelInBracketsDueToMaxHpAdjust(HediffCompProperties_MaxHPAdjust props)
        {
            if (!EliteBionicsFrameworkMain.SettingHandle_DisplayHpDiffInHediffName)
            {
                return "";
            }
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

        public static float GetPartMaxHealthFromPawnmorpher(BodyPartRecord record, Pawn p)
        {
            // we assert that Pawnmorpher is loaded; dont call without checking that Pawnmorpher exists
            PostFix_BodyPart_GetMaxHealth.SuppressNextWarning();
            return Mixed_Pawnmorpher_GetPartMaxHealth.GetPartMaxHealthDueToPawnmorpher(record, p);
        }

        public static List<HediffCompProperties_MaxHPAdjust> GetRealAndFakeHpPropsForUnification(Pawn pawn, BodyPartRecord record, HediffWithComps constraintComps = null)
        {
            List<HediffCompProperties_MaxHPAdjust> realAndFakeProps = new List<HediffCompProperties_MaxHPAdjust>();
            HediffSet hediffSet = pawn.health.hediffSet;

            List<HediffWithComps> hediffList = new List<HediffWithComps>();
            hediffSet.GetHediffs(ref hediffList);
            foreach (HediffWithComps hediffWithComp in hediffList)
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
                    // note: due to CONN officially changing to use EBF directly, we no longer need to check for CONN
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
                // note: due to CONN officially changing to use EBF directly, we no longer need to check for CONN
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

        public static HediffCompProperties_MaxHPAdjust_Fake TryExtractPawnmorpherHediffToFakeHpComp(Pawn pawn, BodyPartRecord record)
        {
            if (Pawnmorpher_Type_MutationUtilities == null || pawn == null /* GetMutationTracker assumes it is not null, so better check */)
            {
                // not loaded
                return null;
            }
            object mutationTracker = Pawnmorpher_Type_MutationUtilities.GetMethod("GetMutationTracker").Invoke(null, [pawn]);
            if (mutationTracker == null) 
            {
                return null;
            }
            IEnumerable<object> allMutations = (IEnumerable<object>)Pawnmorpher_Type_MutationTracker.GetProperty("AllMutations").GetGetMethod().Invoke(mutationTracker, null);
            float pmMultiplier = 0;
            float pmOffset = 0;
            foreach (object mutationHediff in allMutations)
            {
                object mutationStage = Pawnmorpher_Type_HediffAddedMutation.GetProperty("CurrentMutationStage").GetGetMethod().Invoke(mutationHediff, null);
                if (mutationStage != null)
                {
                    float tempVal = (float)Pawnmorpher_Type_MutationStage.GetField("globalHealthMultiplier").GetValue(mutationStage);
                    if (tempVal != 0)
                    {
                        pmMultiplier += tempVal;
                    }
                    // self-note: because mutationHediff extends Hediff (duh), we can cast directly back to Hediff instead of using reflection
                    BodyPartRecord tempObj = ((Hediff) mutationHediff).Part;
                    if (tempObj == record)
                    {
                        pmOffset += (float)Pawnmorpher_Type_MutationStage.GetField("healthOffset").GetValue(mutationStage);
                    }
                }
            }
            if (pmMultiplier == 0 && pmOffset == 0)
            {
                // nothing here
                return null;
            }
            float properMultiplier = (pmMultiplier > 0 ? pmMultiplier : 1);
            // both are essentially multipliers
            HediffCompProperties_MaxHPAdjust_Fake fakeComp = new HediffCompProperties_MaxHPAdjust_Fake
            {
                linearAdjustment = 0,
                scaleAdjustment = (pmOffset + 1) * properMultiplier - 1,
                providerNamespace = Pawnmorpher_Type_MutationUtilities.Namespace
            };
            return fakeComp;
        }
    }
}
