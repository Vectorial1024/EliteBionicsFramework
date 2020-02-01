using HugsLib;
using HugsLib.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EBF
{
    public class EliteBionicsFrameworkMain: ModBase
    {
        public static string MODID => "com.vectorial1024.rimworld.ebf";

        /// <summary>
        /// Already includes a space character.
        /// </summary>
        public static string MODPREFIX => "[V1024-EBF] ";

        public override string ModIdentifier => MODID;

        internal static bool HasAttemptedToFindMoody = false;

        internal static bool MoodyIsRunning = false;

        // Settings 

        public static SettingHandle<bool> SettingHandle_DisplayHpDiffInHediffName { get; private set; }

        public override void DefsLoaded()
        {
            SettingHandle_DisplayHpDiffInHediffName = Settings.GetHandle("displayHpDiffInHediffName", "Display HP diff in hediff name", "If enabled, EBF will display how the max HP of body parts is affected by EBF-enabled hediffs.\n\nEnabled by default.", true);
        }

        public static void LogError(string message, bool ignoreLogLimit = false)
        {
            Log.Error(MODPREFIX + " " + message, ignoreLogLimit);
        }

        public static void LogWarning(string message, bool ignoreLogLimit = false)
        {
            Log.Warning(MODPREFIX + " " + message, ignoreLogLimit);
        }
    }
}
