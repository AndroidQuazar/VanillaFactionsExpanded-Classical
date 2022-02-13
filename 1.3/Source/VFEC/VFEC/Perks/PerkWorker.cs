using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEC.Perks
{
    public class PerkWorker
    {
        private static readonly HashSet<Patch> APPLIED = new();

        // ReSharper disable once InconsistentNaming
        public PerkDef def;

        public PerkWorker(PerkDef def) => this.def = def;

        public IEnumerable<Patch> Patches => PerkPatches.GetPatches().Concat(GetPatches());

        public virtual void Notify_Added()
        {
            foreach (var patch in Patches)
                if (!APPLIED.Contains(patch))
                {
                    Log.Message($"Perk {def.LabelCap} is patching {patch}");
                    try
                    {
                        patch.Apply(ClassicMod.Harm);
                    }
                    catch (Exception error)
                    {
                        Log.Error($"Error while patching for {def.LabelCap}:\n{error}");
                    }

                    APPLIED.Add(patch);
                }
        }

        public virtual bool ShouldModifyStatsOf(StatRequest req, StatDef stat) =>
            def.statFactors is not null && def.statFactors.Any(factor => factor.stat == stat) || def.statOffsets is not null && def.statOffsets.Any(offset => offset.stat == stat);

        public virtual void Notify_Removed()
        {
            foreach (var patch in Patches)
                if (APPLIED.Contains(patch))
                {
                    patch.Unapply(ClassicMod.Harm);
                    APPLIED.Remove(patch);
                }
        }

        public virtual IEnumerable<Patch> GetPatches()
        {
            yield break;
        }

        public struct Patch
        {
            private readonly MethodInfo target;
            private readonly MethodInfo prefix;
            private readonly MethodInfo postfix;
            private readonly MethodInfo transpiler;
            public override string ToString() => $"{target} with:\nPrefix: {prefix}\nPostfix: {postfix}\nTranspiler: {transpiler}";

            public static Patch Prefix(MethodInfo target, MethodInfo prefix) => new(target, prefix);
            public static Patch Postfix(MethodInfo target, MethodInfo postfix) => new(target, postfix: postfix);
            public static Patch Transpiler(MethodInfo target, MethodInfo transpiler) => new(target, transpiler: transpiler);

            public Patch(MethodInfo target, MethodInfo prefix = null, MethodInfo postfix = null, MethodInfo transpiler = null)
            {
                this.target = target;
                this.prefix = prefix;
                this.postfix = postfix;
                this.transpiler = transpiler;
            }

            public void Apply(Harmony harm)
            {
                harm.Patch(target,
                    prefix is null ? null : new HarmonyMethod(prefix),
                    postfix is null ? null : new HarmonyMethod(postfix),
                    transpiler is null ? null : new HarmonyMethod(transpiler));
            }

            public void Unapply(Harmony harm)
            {
                if (prefix is not null) harm.Unpatch(target, prefix);

                if (postfix is not null) harm.Unpatch(target, postfix);

                if (transpiler is not null) harm.Unpatch(target, transpiler);
            }
        }
    }
}