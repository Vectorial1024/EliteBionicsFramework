# Elite Bionics Framework (EBF)
A common framework to change body part max HP.

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

# Usage by Other Mods

This mod is now being used by a few other mods to boost body part max HP values.

For more info and an updated list of mods, check out the relevant Steam Collection here: https://steamcommunity.com/workshop/filedetails/?id=2016834921

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

For more info and an updated list of mods, check out the relevant Steam Collection here: https://steamcommunity.com/workshop/filedetails/?id=2693300854

# Natural Compatibility:

The following mods, because of technical reasons, are perfectly compatible with EBF, and no action is required from EBF's side unless they made a breaking update:

- Vanilla Factions Expanded - Insectoids (? feels like previously certified but forgot where)

# Community Unification

This mod has built-in unification with some other third-party mods that also provides body part HP changing effects. This is only to allow those mods to work with EBF without issues, and I do not claim ownership to those mods.

For more info and an updated list of mods, check out the relevant Steam Collection here: https://steamcommunity.com/workshop/filedetails/?id=2828792658

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

# About "adopting the EBF protocol"
Sometinmes, I am simply unavailable. And sometimes, some other mod gained enough popularity but I did not notice them. Such is the way of things, but this may result in broken compatibility. This may occur in the form of "EBF Protocol Violation" notices:

```
[V1024-EBF] Elite Bionics Framework has detected some mods using the unmodified GetMaxHealth() method, which violates the EBF protocol. 
The author(s) of the involved mod(s) should adopt the EBF to clarify their intentions.
For now, the unmodified max HP is returned.
The detected mod comes from: [name]
```

This is mainly due to this mod adding extra information to body parts, so that only having a `BodyPartDef` is no longer enough to determine the max HP. You need to pass in a `BodyPartRecord` instance, so that EBF may know how to calculate the max HP.

You should differentiate between the following:
- Calculating the current max HP, which may be affected by EBF bionics (note: most use cases are this case)
- Checking the base stats of the body part, which is never affected by EBF, and with which EBF bases its max HP calculation on

EBF has provided compatiblity by transpiling various other mods' DLLs via Harmony to use the EBF-compliant methods, but the best way to do this would be to write the correct code at those DLLs instead, so that the intention is the clearest.

In case you want to calculate the current max HP, I will not be providing exact code, but consider the following:

1. Create a utility function that has the following signature to calculate the max HP of a body part: `function(BodyPartRecord, Pawn) -> float`
2. Make the body of said function return the vanilla max HP: `return BodyPartRecord->def::GetMaxHealth(Pawn)`
3. Make a Harmony prefix patch targetting said function:
```
Prepare:
    EBF is loaded

TargetMethod:
    [your util function]

bool PreFix(ref float __result):
    (note: use reflection if needed)
    __result = EBFEndpoints.GetMaxHealthWithEBF(BodyPartRecord, Pawn);
    return false; // skip the original method
```

This will allow you to read the current max HP value while not triggering the "adopt the EBF protocol" error.
