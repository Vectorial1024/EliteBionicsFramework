# Elite Bionics Framework (EBF)
A common framework to change body part max HP.

## Installing this mod
The recommended way to install this mod is through the Steam Workshop: https://steamcommunity.com/sharedfiles/filedetails/?id=1665403571

Pre-packed archives are sometimes available at GitHub releases: https://github.com/Vectorial1024/EliteBionicsFramework/releases

Still, advanced users and other modders may want to clone this repo with Git, to access latest/development builds.

## Third-party mod integrations/interactions
There are several ways the Elite Bionics Framework ("EBF") interacts with mods made by others:
- Some mods depend on the EBF to enhance their bionics
  - See this Steam Workshop Collection for known items: https://steamcommunity.com/workshop/filedetails/?id=2016834921
- Some mods are patched to read updated statistics from EBF
  - See this Steam Workshop Collection for known items: https://steamcommunity.com/workshop/filedetails/?id=2693300854
- Some mods are patched to make their max-HP boosts work correctly
  - See this Steam Workshop Collection for known items: https://steamcommunity.com/workshop/filedetails/?id=2828792658
  - The main criteria of selecting mods to manually patch is their stability
  - EBF does not claim ownership to those mods; the patching is done only to fix bugs

------

# Appendix: Adding compatibility with this mod
There are several ways to add compatibility with this mod, which corresponds to the 3 interaction categories listed above:
- For XML modders to enhance/buff their bionics/hediffs
- For C# modders to read the correct `BodyPartRecord` max HP
- For C# modders to programmatically enhance/buff their bionics/hediffs (advanced usage!)

> [!TIP]
> The Elite Bionics Framework is most commonly used to buff bionics, but the framework works on basically any Hediff.
>
> One possible creative use is to make a Potion of Toughness that temporarily buffs the max HP of a given body part.

## Appendix 1: XML modders and buffing bionics/hediffs
The EBF is available as additional `HediffComp`s to your bionics/hediffs. This section will only show the end result after adding EBF; this section will not teach you how to do XML patching.

> [!IMPORTANT]
> For XML patching, be aware that the game rejects the following:
> - multiple `<comps>` nodes under the same `<HediffDef>` node
> - multiple `<li>` nodes under the same `<comps>` node with the same `Class="..."` value
>
> These are the game's limitation/requirements, and have nothing to do with Elite Bionics Framework. Be defensive when writing XML patches!

> [!TIP]
> As of RimWorld 1.1, you may use the atttribute `MayRequire="..."` to make EBF support available but optional.

A `HediffDef` XML code excerpt that uses `MayRequire="..."` is as follows:

```xml
<HediffDef>
    <!-- hediff name, hediff worker class, etc. -->
    <comps>
        <li class="EBF.Hediffs.HediffCompProperties_MaxHPAdjust" MayRequire="V1024.EBFramework">
            <linearAdjustment>20</linearAdjustment>
            <scaleAdjustment>0.15</scaleAdjustment>
        </li>
    <comps>
    <!-- other hediff-related code, etc. -->
</HediffDef>
```

More information about available XML components are found inside the `/Docs` folder.

## Appendix 2: C# modders and reading statistics from EBF
The updated statistics values can be read from the class `EBF.EBFEndpoints`. Please refer to the source code in this repo for the latest method details (they rarely change and can be safely accessed with reflection). 

> [!TIP]
> As of Harmony 2 (i.e. RimWorld 1.2), You can make integration easier by utilizing reverse-patching.

A sample C# code excerpt making use of reverse-patching from the trusty Harmony 2 is as follows:

```c#
[HarmonyPatch]
public class MaxHealthGetter
{
    public static bool Prepare()
    {
        // detect whether the EBF is loaded
        return LoadedModManager.RunningMods.Where((ModContentPack pack) => pack != null && pack.ModMetaData.Active && pack.PackageId == "V1024.EBFramework");
    }

    public static MethodBase TargetMethod()
    {
        // the correct EBF endpoint method to get the updated statistics
        return AccessTools.Method("EBF.EBFEndpoints:GetMaxHealthWithEBF");
    }

    [HarmonyReversePatch]
    public static float GetMaxHealth(BodyPartRecord record, Pawn pawn)
    {
        // if EBF is loaded, then Harmony replaces the body with the EBF endpoint method
        // else, the reverse-patch fails and the body remains the vanilla GetMaxHealth().
        return record.def.GetMaxHealth(pawn);
    }
}
```

### About "adopting the EBF protocol"
Calling the vanilla `Verse.BodyPartDef:GetMaxHealth(Pawn)` method while this mod is active will emit a warning that looks like this:

```
[V1024-EBF] Elite Bionics Framework has detected some mods using the unmodified GetMaxHealth() method, which violates the EBF protocol. 
The author(s) of the involved mod(s) should adopt the EBF to clarify their intentions.
For now, the unmodified max HP is returned.
The detected mod comes from: [name]
```

The reason for this warning is that, under EBF, only having a `BodyPartDef` is not enough to determine the correct body part max HP when bionics are installed or `Hediff`s are added. As an example, a colonist have two `BodyPartRecord`s (left shoulder and right shoulder) but these share the same `BodyPartDef`.

If you see this warning, do report this to both the EBF and to the mod authors involved, so that compatibility may be ensured.

If you are a C# modder seeing this warning, then you should clarify your intentions by replacing the vanilla method call with one of the following:
- `EBF.EBFEndpoints:GetMaxHealthUnmodified(BodyPartDef, Pawn)`: returns the def-backed max HP value, suppressing this warning
- `EBF.EBFEndpoints:GetMaxHealthWithEBF(BodyPartRecord, Pawn)`: returns the EBF-calculated max HP value of a body part record

## Appendix 3: C# modders and writing effects to EBF
This is advanced usage; it entails programmatically providing the max HP adjustment to EBF. Do review whether the same effect may be achieved by modifying XML files since that method is usually simpler.

Still, as of EBF 6.0, it is possible to programmatically provide max HP adjustment to EBF. EBF will then pick up the value, and include it in the max HP calculations.

A NuGet package is required: `Vectorial1024.EliteBionicsFrameworkAPI`.

At your `HediffComp`, implement `EBF.API.IHediffCompAdjustsMaxHp`. In particular, provide your intended max HP adjustment through the abstract property `public BodyPartMaxHpAdjustment MaxHpAdjustment`, so EBF may read it and know how you intend to adjust the max HP values.

> [!IMPORTANT]
> Currently, EBF caches body part max HP values, so at the moment, it is not possible to dynamically change max HP adjustment based on custom events (hypothetical e.g. bionics from CyberNet losing the max HP bonus when the GlitterNet goes out).
>
> However, support for dynamic HP max adjustment is being considered.
