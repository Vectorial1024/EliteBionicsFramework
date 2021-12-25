# Elite Bionics Framework (EBF)
A common framework to change body part max HP.

Special Note: due to an unexplainable incompatibility with Humanoid Alien Races, I have made the following changes: EBF hediffs will no longer influence the chance of pawns getting stunned after taking blunt damage; i.e., the chance of getting stunned from blunt damage is now the same as vanilla calculation.

# Downloading this Mod (Normal Users)
1. Go back to https://github.com/Vectorial1024/EliteBionicsFramework
2. Notice the "Clone or Download" button on the top right hand corner (under the "1 Contributor" area). Click on it and click "Download as ZIP"
3. The ZIP should conain 1 folder. Unpack the ZIP, and place the folder in the /Mods directory of your RimWorld installation.

# Downloading this Mod (Advanced Users)
The main idea is to git-clone the repo to local so that you could git-pull regularly to apply updates (if exists)
1. To "build", do:
```
# cd to RimWorld root directory first
mkdir Mods
cd Mods
git clone https://github.com/Vectorial1024/EliteBionicsFramework.git
```
2. To update to latest, do:
```
git pull
```

# Built-In Integration with Other Mods

This mod has built-in integration with some other third-party mods, and such integrations are in place to ensure that those other mods can read the correct body part max HP as calculated by this framework mod.

The third-party mods are chosen because they are deemed stable enough that the IL-code patching technique utilized by this mod can easily survive long without problems; or, they are chosen because not doing so would break some features.

Currently, the following mods are supported, in no particular order:

- Humanoid Alien Races
- Pawnmorpher
- EdB Prepare Carefully
- CM Callouts
- Moody

Note that this list does not include the mods that are using this framework to provide actual body part max HP manipulation.

# Usage and Other Related Info

Please refer to individual md files under `/Docs` for detailed information of each available component.

Currently, these components are available:

- HediffCompProperties_MaxHPAdjust
- HediffCompProperties_VerbPowerAdjust

# Appendix: Adding HediffCompProperties to your Hediff
In RimWorld, Hediffs may optionally carry HediffCompProperties to add flavour to the Hediff. Examples of HediffCompProperties already implemented in RimWorld are `HediffCompProperties_Immunizable` (used by most, if not all, diseases and infections), `HediffCompProperties_VerbGiver` (used by some bionics and other mods to e.g. increase body part melee damage; there may be other uses) and `HediffCompProperties_Disappears` (used by Fibrous/Sensory Mechanite Hediffs)

`HediffCompProperties` are representatives of `HediffComps`. `HediffCompProperties` lets us define parameters for `HediffComps` to use, so `HediffComps` can show our intended effect.

To add `HediffCompProperties` to your `Hediff`, open the XML file that contains your `Hediff`, and add an `li` entry to the `comps` node. If the `comps` node does not exist, add it first.

## Example XML 1
After adding your `HediffCompProperties`, your `Hediff` XML Def should look something like this:
```XML
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
```XML
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