using EBF.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace EBF.Transpilations
{
    [HarmonyPatch]
    public static class Transpiler_DamageWorker_Blunt_LambdaOne
    {
        public static bool Prepare()
        {
            bool available = TargetMethod() != null;
            if (!available)
            {
                EliteBionicsFrameworkMod.LogError("Failed to detect target self-anon type for patching: blunt lambda one");
            }
            return available;
        }

        public static MethodBase TargetMethod()
        {
            // (we will let this print error when it fails to detect things... somehow we cannot properly encapsulate the error correctly)

            // RW v1.6, FQ name Verse.DamageWorker_Blunt/'<>c'::'<>9__2_0'
            // weak matching: this self-anon type has 9 fields
            var potentialSelfAnons = AccessTools.TypeByName("Verse.DamageWorker_Blunt").GetNestedTypes(BindingFlags.NonPublic).Where((Type type) => type.GetFields().Length == 9);
            if (potentialSelfAnons.Count() == 0)
            {
                EliteBionicsFrameworkMod.LogError("field-count criteria fail");
                return null;
            }
            Type typeSelfAnon = potentialSelfAnons.First();
            // and then, within these 9 fields, pick the one with signature (ThingDef, float32, bool) -> string
            foreach (MethodInfo info in typeSelfAnon.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                // weak match: this is the only method that has this many parameters
                if (info.GetParameters().Length == 3)
                {
                    // is this one!
                    return info;
                }
            }
            // could not find it
            EliteBionicsFrameworkMod.LogError("field-count criteria fit, signature check fail");
            return null;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * A total of 1 GetMaxHealth occurences detected;
             * Patch with CodeMatcher
             */
            return new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth)))
                ) // find the only occurence of .GetMaxHealth()
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingDef), "race")),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RaceProperties), "body")),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BodyDef), "corePart")),
                    new CodeInstruction(OpCodes.Call, VanillaExtender.ReflectionGetMaxHealth())
                ) // insert extra code so that we use VanillaExtender.GetMaxHealth(); we do this out of convenience
                .Set(OpCodes.Nop, null) // and ignore the original instruction
                .InstructionEnumeration();
        }
    }
}
