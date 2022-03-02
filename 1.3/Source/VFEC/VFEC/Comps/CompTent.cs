using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEC.Comps
{
    [StaticConstructorOnStartup]
    public class CompTent : ThingComp
    {
        public static bool SkipRendering;
        private static bool donePatches;

        private static HashSet<ThingDef> tents = new();

        public static IEnumerable<CodeInstruction> RenderPawnAt_Transpile(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var labelNext = false;
            var label = generator.DefineLabel();
            foreach (var instruction in instructions)
            {
                if (labelNext)
                {
                    yield return instruction.WithLabels(label);
                    labelNext = false;
                }
                else yield return instruction;

                if (instruction.opcode == OpCodes.Stloc_S && instruction.operand is LocalBuilder {LocalIndex: 8})
                {
                    labelNext = true;
                    yield return CodeInstruction.LoadField(typeof(CompTent), nameof(SkipRendering));
                    yield return new CodeInstruction(OpCodes.Brfalse, label);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return CodeInstruction.StoreField(typeof(CompTent), nameof(SkipRendering));
                    yield return new CodeInstruction(OpCodes.Ret);
                }
            }
        }

        public static IEnumerable<CodeInstruction> GetBodyPos_Transpile(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var labelNext = false;
            var label = generator.DefineLabel();
            foreach (var instruction in instructions)
            {
                if (labelNext)
                {
                    yield return instruction.WithLabels(label);
                    labelNext = false;
                }
                else yield return instruction;

                if (instruction.opcode == OpCodes.Stind_I1)
                {
                    labelNext = true;
                    yield return CodeInstruction.LoadField(typeof(CompTent), nameof(tents));
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return CodeInstruction.LoadField(typeof(Thing), nameof(Thing.def));
                    yield return CodeInstruction.Call(typeof(HashSet<ThingDef>), nameof(HashSet<ThingDef>.Contains));
                    yield return new CodeInstruction(OpCodes.Brfalse, label);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                    yield return CodeInstruction.StoreField(typeof(CompTent), nameof(SkipRendering));
                    yield return CodeInstruction.Call(typeof(Vector3), "get_zero");
                    yield return new CodeInstruction(OpCodes.Ret);
                }
            }
        }

        public override void Notify_AddBedThoughts(Pawn pawn)
        {
            base.Notify_AddBedThoughts(pawn);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOutside);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOnGround);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInCold);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInHeat);
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            if (!tents.Contains(parent.def)) tents.Add(parent.def);
            if (!donePatches)
            {
                ClassicMod.Harm.Patch(AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.RenderPawnAt)),
                    transpiler: new HarmonyMethod(GetType(), nameof(RenderPawnAt_Transpile)));
                ClassicMod.Harm.Patch(AccessTools.Method(typeof(PawnRenderer), "GetBodyPos"), transpiler: new HarmonyMethod(GetType(), nameof(GetBodyPos_Transpile)));
                donePatches = true;
            }
        }
    }
}