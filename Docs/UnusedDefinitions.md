*The things here are not necessarily wrong, just that I find it too wasteful to just discard them, so I left them here. Consider this as the alternative, more verbose version of the HediffCompProperties Case 2 part.*

Further modding the Bionic Arms with additional `HediffCompProperties_ToolPowerAdjust` will be

## Tool Power Adjuster Comp with non-implant Hediff

This is a bit complicated. Some defintions and graphs are needed to illustrate the point.

You may skip to the examples in the next big section if you want.

First, let's have a look at how this mod organizes the relationship between `BodyPartRecord`s, `BodyPartGroupDef`s, and `Tool`s.

Normally, `Tool`s are organized as direct childrens of `Pawn`s inside the `PawnDef`, but for convenience, this mod organizes `Tool`s as the youngest childrens of the `BodyPartRecord` tree of a `Pawn`. Thus, a Colonist's "tool-tree" may look like this:

![A simplified "tool-tree" of a Colonist](Graph01_BodyPartTree.png)

Over time, Colonists may receive bionic parts as the player wishes. Let's say that the player installed a vanilla bionic arm at left shoulder. Then, focusing on the left shoulder, the "tool-tree" becomes like this:

![A simplified "tool-tree" of a Colonist having a bionic arm](Graph02_BionicInShoulder.png)

Notice that the `Tool` under the `HediffComp_VerbGiver` looks just like the `Tool` under the original `BodyPartGroupDef` object. This is what this mod aims to determine.

### Definition 1
A `Tool` is said to be *comparable* with another `Tool` if the list of `tool capacities` of both tools are the same. Otherwise, they are said to be *incomparable*.

The defintion uses the list concept to be applicable in general cases (e.g., muffalo hoofs can both `Blunt` and `Poke`, while human fists can only `Blunt`).

### Example 1
Given these two `Tool`s:

- Tool A (`tool capacities`: `Blunt` only)
- Tool B (`tool capacities`: `Blunt` only)

Tool A and Tool B are comparable.

### Example 2
Given these two `Tool`s:

- Tool A (`tool capacities`: `Blunt` and `Poke`)
- Tool B (`tool capacities`: `Blunt` and `Poke`)

Tool A and Tool B are comparable.

### Example 3
Given these two `Tool`s:

- Tool A (`tool capacities`: `Blunt` only)
- Tool B (`tool capacities`: `Blunt` and `Poke`)

Tool A and Tool B are NOT comparable; they are incomparable.

### Definition 2
A `Tool` is said to *belong to* a `BodyPartRecord` (or a `Hediff`) if, by generating the "tool-table", the `Tool` appears under the `BodyPartRecord` (or the `Hediff`).

Let us see some examples.

### Example 4
Given this "tool-tree" of a unmodified Colonist:

![A simplified "tool-tree" of a Colonist](Graph01_BodyPartTree.png)

These statements are true:
- "Left fist" belongs to "Left shoulder"
- "Left fist" belongs to "Torso" (not that anyone would be interested in this interpretation)
- "Teeth (`Tool`)" belongs to "Jaw"

These statements are false:
- "Left fist" belongs to "Right shoulder"
- "Right fist" belongs to "Jaw"
- Melee weapons (equipped or not) belong to "Torso"

### Example 5
Given this "tool-tree" of a Colonist who has received a bionic arm at their left shoulder:

![A simplified "tool-tree" of a Colonist having a bionic arm](Graph02_BionicInShoulder.png)

These statements are true:
- "Left fist" belongs to "Left shoulder"
- "Fist" of `HediffComp_VerbGiver` belongs to "Left shoulder"

### Definition 3
A `Tool` is said to be *correspondable to* another `Tool` if

- both `Tool`s belong to the same `BodyPartRecord`; and
- both `Tool`s are comparable with each other

### Example 6
Given this "tool-tree" of a Colonist who has received a bionic arm at their left shoulder:

![A simplified "tool-tree" of a Colonist having a bionic arm](Graph02_BionicInShoulder.png)

This statements is true:
- "Fist" of `HediffComp_VerbGiver` is correspondable to "Left fist" at "Left shoulder"

## Examples of Case 2 (and General Walkthrough of using HediffCompProperties_ToolPowerAdjust)
Let's say we are going to add a `HediffCompProperties_ToolPowerAdjust` to the vanilla Bionic Arm hediff for Colonists.

The Bionic Arm hediff XML should look like this:

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
    </comps>
    <!-- Omitted code -->
</HediffDef>
```

Note that Bionic Arms, unless modded, can only be installed on Colonists' shoulders (e.g. left shoulder). By definition 2, this means that installed Bionic Arms always *belong to* the installed shoulder (`[1]`).

Let's also say you want to reconfigure the Bionic Arm to give a flat bonus of +4, thinking it as some "additional, +4 strength" instead of "new 12 strength".

Check the `PawnDef` XML, and you notice something like this:

```XML
<tools>
    <li>
        <label>left fist</label>
        <capacities>
            <li>Blunt</li>
        </capacities>
        <power>8.2</power>
        <cooldownTime>2</cooldownTime>
        <linkedBodyPartsGroup>LeftHand</linkedBodyPartsGroup>
        <!-- Omitted code -->
    </li>
<!-- Omitted code -->
</tools>
```

By defintion 2, the left fist tool also belongs to shoulders (`[2]`).

The bionic fist tool has capacity {`Blunt`}, while the left fist tool has capacity {`Blunt`}, which is the same as the bionic fist tool. By defintion 1, the bionic fist tool is *comparable with* the left fist tool (`[3]`).

Given `[1]`, `[2]`, and `[3]`, by definition 3, the bionic fist tool is *correspondable to* the left fist tool at left shoulder. **As a result, the Framework determines that any `HediffCompProperties_ToolPowerAdjust` added to the Bionic Arm is effective.**

We are mostly done!

To set up the +4 bonus, simply modify the Bionic Arm XML into something like this:

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
                    <!-- This becomes a dummy value, and is not used... -->
                    <power>12</power>
                    <cooldownTime>2</cooldownTime>
                </li>
            </tools>
        </li>
        <!-- ...if you provide this and the Framework determines that it is effective. -->
        <li Class="HediffCompProperties_ToolPowerAdjust">
            <linearAdjustment>4</linearAdjustment>
        </li>
    </comps>
    <!-- Omitted code -->
</HediffDef>
```

Then, whenever a Colonist with the modified Bionic Arm melee-attacks another pawn bare-handed, this framework will adjust the fist damage to `8.2 + 4 = 12.2` damage. We are done!