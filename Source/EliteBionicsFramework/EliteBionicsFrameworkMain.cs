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
        public static string MODSHORTID => "V1024-EBF";

        public override string LogIdentifier => MODSHORTID;

        /// <summary>
        /// Already includes a space character.
        /// </summary>
        public static string MODPREFIX => "[" + MODSHORTID + "] ";

        // Settings 

        public static SettingHandle<bool> SettingHandle_DisplayHpDiffInHediffName { get; private set; }

        public override void DefsLoaded()
        {
            SettingHandle_DisplayHpDiffInHediffName = Settings.GetHandle("displayHpDiffInHediffName", "Display HP diff in hediff name", "If enabled, EBF will display how the max HP of body parts is affected by EBF-enabled hediffs.\n\nEnabled by default.", true);
        }

        public static void LogError(string message)
        {
            Log.Error(MODPREFIX + " " + message);
        }

        public static void LogWarning(string message)
        {
            Log.Warning(MODPREFIX + " " + message);
        }
    }
}
