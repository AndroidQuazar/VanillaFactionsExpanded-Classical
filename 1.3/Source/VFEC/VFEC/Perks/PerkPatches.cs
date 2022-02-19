using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEC.Perks
{
    public static class PerkPatches
    {
        public static IEnumerable<PerkWorker.Patch> GetPatches()
        {
            yield return PerkWorker.Patch.Postfix(
                AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized)),
                AccessTools.Method(typeof(PerkPatches), nameof(PerkModifyStat))
            );
            yield return PerkWorker.Patch.Postfix
            (
                AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized)),
                AccessTools.Method(typeof(PerkPatches), nameof(PerkModifyExplanation))
            );
        }

        public static void PerkModifyStat(StatDef ___stat, StatRequest req, ref float __result)
        {
            foreach (var perk in GameComponent_PerkManager.Instance.ActivePerks.Where(perk =>
                (perk.statFactors != null || perk.statOffsets != null) && perk.Worker.ShouldModifyStatsOf(req, ___stat)))
            {
                if (perk.statFactors != null) __result *= perk.statFactors.GetStatFactorFromList(___stat);
                if (perk.statOffsets != null) __result += perk.statOffsets.GetStatOffsetFromList(___stat);
            }
        }

        public static void PerkModifyExplanation(StatWorker __instance, StatDef ___stat, StatRequest req, ref string __result)
        {
            foreach (var perk in GameComponent_PerkManager.Instance.ActivePerks.Where(perk =>
                (perk.statFactors != null || perk.statOffsets != null) && perk.Worker.ShouldModifyStatsOf(req, ___stat)))
            {
                if (perk.statFactors != null)
                {
                    var factor = perk.statFactors.GetStatFactorFromList(___stat);
                    if (Math.Abs(factor - 1f) > 0.0001f)
                        __result += "\n" + (perk.LabelCap + ": " + __instance.ValueToString(factor, false, ToStringNumberSense.Factor));
                }

                if (perk.statOffsets != null)
                {
                    var offset = perk.statOffsets.GetStatOffsetFromList(___stat);
                    if (offset != 0f)
                        __result += "\n" + (perk.LabelCap + ": " + __instance.ValueToString(offset, false, ToStringNumberSense.Offset));
                }
            }
        }
    }
}