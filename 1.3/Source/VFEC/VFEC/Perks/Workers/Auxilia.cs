using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEC.Perks.Workers
{
    public class Auxilia : PerkWorker
    {
        public Auxilia(PerkDef def) : base(def)
        {
        }

        public override IEnumerable<Patch> GetPatches()
        {
            yield return Patch.Postfix(AccessTools.Method(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker"), AccessTools.Method(GetType(), nameof(DoSupportLegion)));
        }

        public static void DoSupportLegion(IncidentWorker_RaidEnemy __instance, IncidentParms parms, ref bool __result)
        {
            if (__result && Rand.Chance(0.25f))
            {
                parms.faction = Find.FactionManager.FirstFactionOfDef(VFEC_DefOf.VFEC_WesternRepublic);
                __result = __result && IncidentDefOf.RaidFriendly.Worker.TryExecute(parms);
            }
        }
    }
}