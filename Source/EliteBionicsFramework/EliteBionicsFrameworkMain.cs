using HugsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EliteBionicsFramework
{
    public class EliteBionicsFrameworkMain: ModBase
    {
        public static string MODID => "com.vectorial1024.rimworld.ebf";

        /// <summary>
        /// Already includes a space character.
        /// </summary>
        public static string MODPREFIX => "[V1024-EBF] ";

        public override string ModIdentifier => MODID;

        public static void LogError(string message, bool ignoreLogLimit = false)
        {
            Log.Error(MODPREFIX + " " + message, ignoreLogLimit);
        }
    }
}
