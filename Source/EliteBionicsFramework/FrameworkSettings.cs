using UnityEngine;
using Verse;

namespace EBF
{
    public class FrameworkSettings : ModSettings 
    {
        public bool showHpDiffInHediffName = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref showHpDiffInHediffName, "showHpDiffInHediffName", true);
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            // since we no longer rely on HugsLib, we gotta do the UI ourselves
            Listing_Standard listing = new Listing_Standard();

            // ...
            listing.verticalSpacing = 8; // make it more clickable/readable
            listing.Begin(inRect);

            /*
             * concept:
             * single-column config that shows a single boolean switch
             */

            listing.CheckboxLabeled("Display HP diff in hediff name", ref showHpDiffInHediffName, tooltip: "If enabled, EBF will display how the max HP of body parts is affected by EBF-enabled hediffs.\n\nEnabled by default.");

            // all done
            listing.End();
        }
    }
}
