# Elite Bionics Framework (EBF)
A common framework to change body part max HP.

# Downloading this Mod (Normal Users)
1. Go back to https://github.com/Vectorial1024/EliteBionicsFramework
2. Notice the "Clone or Download" button on the top right hand corner (under the "1 Contributor" area). Click on it and click "Download as ZIP"
3. The ZIP should conain 1 folder. Unpack the ZIP, and place the folder in the /Mods directory of your RimWorld installation.

# Usage and Other Related Info
The EBF introduces `HediffCompProperties_MaxHPAdjust` as a common `HediffCompProperties` for other modders to designate body part max HP changes for their `Hediff`s. Please refer to the appendix below for learning how to add `HediffCompProperties` to your `Hediff` in general.

Please note that the full designation of the Hediff is `EBF.Hediffs.HediffCompProperties_MaxHPAdjust`. Make sure you get this correct.

Under EBF, the formula to calculate a body part's max HP is:
`maxHpFromDef * pawn.bodySize + linearAdjustment`
Note that `maxHpFromDef * pawn.bodySize` is how vanilla calculates a body part's max HP.

There is currently 1 property under `HediffCompProperties_MaxHPAdjust`.
## `linearAdjustment`
Takes an integer: the `Hediff` will offset the body part max HP by this value

Note: the offset is applied *after* the consideration of the pawn's body size. For example, if you stated `+30 linearAdjustment`, and applied the `Hediff` to a Colonist and a Thrumbo, you will see that both the Colonist and the Thrumbo will get the same "+30 max HP" buff.

# Appendix: Adding HediffCompProperties to your Hediff
In RimWorld, Hediffs may optionally carry HediffCompProperties to add flavour to the Hediff. Examples of HediffCompProperties already implemented in RimWorld are `HediffCompProperties_Immunizable` (used by most, if not all, diseases and infections), `HediffCompProperties_VerbGiver` (used by some bionics and other mods to e.g. increase body part melee damage; there may be other uses) and `HediffCompProperties_Disappears` (used by Fibrous/Sensory Mechanite Hediffs)

`HediffCompProperties` are representatives of `HediffComps`. `HediffCompProperties` lets us define parameters for `HediffComps` to use, so `HediffComps` can show our intended effect.

To add `HediffCompProperties` to your `Hediff`, open the XML file that contains your `Hediff`, and add an `li` entry to the `comps` node. If the `comps` node does not exist, add it first.

## Example XML 1
After adding your `HediffCompProperties`, your `Hediff` XML Def should look something like this:
```
<HediffDef>
    <!-- Omitted code -->
    <comps>
        <!-- Adding HediffCompProperties_VerbGiver -->
        <li Class="HediffCompProperties_VerbGiver">
            <tools>
                <li>
                    <label>fist</label>
                    <capacities>
                        <li>Blunt</li>
                    </capacities>
                    <power>12</power>
                    <cooldownTime>2</cooldownTime>
                </li>
            </tools>
        </li>
        <!-- Add complete -->
    </comps>
    <!-- Omitted code -->
</HediffDef>
```

## Example XML 2
If you want to add in more `HediffCompProperties`, just add in additional `li` nodes like this:
```
<HediffDef>
    <!-- Omitted code -->
    <comps>
        <li Class="HediffCompProperties_VerbGiver">
            <tools>
                <li>
                    <label>fist</label>
                    <capacities>
                        <li>Blunt</li>
                    </capacities>
                    <power>12</power>
                    <cooldownTime>2</cooldownTime>
                </li>
            </tools>
        </li>
        <!-- Adding HediffCompProperties_MaxHPAdjust -->
        <li Class="EBF.Hediffs.HediffCompProperties_MaxHPAdjust">
            <linearAdjustment>40</linearAdjustment>
        </li>
        <!-- Add complete -->
    </comps>
    <!-- Omitted code -->
</HediffDef>
```