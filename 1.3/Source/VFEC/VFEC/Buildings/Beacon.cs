using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VFEC.Buildings
{
    [StaticConstructorOnStartup]
    public class Beacon : Building, ISizeReporter
    {
        private static readonly Texture2D LightTex = ContentFinder<Texture2D>.Get("UI/Gizmos/LightTheBeacon");

        private int litTick = -1;

        private MapComponent_Beacon mapComp;

        private Sustainer sustainer;
        private int ticksTillSmoke = -1;

        public float CurrentSize() => 300f;

        public override IEnumerable<Gizmo> GetGizmos() => base.GetGizmos().Append(LightCommand());

        private Faction GetFaction()
        {
            var faction = Find.FactionManager.FirstFactionOfDef(VFEC_DefOf.VFEC_WesternRepublic);
            if (faction.PlayerRelationKind == FactionRelationKind.Ally) return faction;

            faction = Find.FactionManager.FirstFactionOfDef(VFEC_DefOf.VFEC_CentralRepublic);
            if (faction.PlayerRelationKind == FactionRelationKind.Ally) return faction;

            faction = Find.FactionManager.FirstFactionOfDef(VFEC_DefOf.VFEC_EasternRepublic);
            if (faction.PlayerRelationKind == FactionRelationKind.Ally) return faction;
            faction = Find.FactionManager.RandomAlliedFaction();
            return faction;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            mapComp = map.GetComponent<MapComponent_Beacon>();
        }

        private Command LightCommand()
        {
            var command = new Command_Action
            {
                defaultLabel = "VFEC.LightTheBeacon".Translate(),
                defaultDesc = "VFEC.LightTheBeacon.Desc".Translate(),
                icon = LightTex,
                action = delegate
                {
                    litTick = Find.TickManager.TicksGame;
                    mapComp.Notify_BeaconLit();
                    IncidentDefOf.RaidFriendly.Worker.TryExecute(GetIncidentParms());
                }
            };

            if (!mapComp.CanLightBeacon()) command.Disable("VFEC.Cooldown".Translate());
            else if (GetFaction() is null) command.Disable("VFEC.NoAllies".Translate());
            // else if (!IncidentDefOf.RaidFriendly.Worker.CanFireNow(GetIncidentParms())) command.Disable("VFEC.NoReinforce".Translate());

            return command;
        }

        private IncidentParms GetIncidentParms() =>
            new()
            {
                faction = GetFaction(),
                target = Map,
                points = StorytellerUtility.DefaultThreatPointsNow(Map)
            };

        public override void Draw()
        {
            DrawAt(DrawPos);
            if (litTick > 0) Comps_PostDraw();
        }

        public override void Tick()
        {
            base.Tick();
            if (litTick > 0)
            {
                if (ticksTillSmoke <= 0)
                {
                    FleckMaker.ThrowSmoke(DrawPos, Map, 3f);
                    FleckMaker.ThrowFireGlow(Position.ToVector3Shifted(), Map, 0.01f);
                    ticksTillSmoke = Mathf.RoundToInt(10f * Rand.Value);
                }

                ticksTillSmoke--;

                if (sustainer != null)
                    sustainer.Maintain();
                else if (!Position.Fogged(Map))
                {
                    var info = SoundInfo.InMap(new TargetInfo(Position, Map), MaintenanceType.PerTick);
                    sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, SoundDefOf.FireBurning, info);
                }

                if (Find.TickManager.TicksGame >= litTick + MapComponent_Beacon.BEACON_COOLDOWN) Destroy(DestroyMode.KillFinalize);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref litTick, "litTick");
            Scribe_Values.Look(ref ticksTillSmoke, "ticksTillSmoke");
        }
    }

    public class MapComponent_Beacon : MapComponent
    {
        public const int BEACON_COOLDOWN = 180000;
        private int lastBeaconTick = -BEACON_COOLDOWN * 2;

        public MapComponent_Beacon(Map map) : base(map)
        {
        }

        public void Notify_BeaconLit()
        {
            lastBeaconTick = Find.TickManager.TicksGame;
        }

        public bool CanLightBeacon() => lastBeaconTick + BEACON_COOLDOWN <= Find.TickManager.TicksGame;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastBeaconTick, "lastBeaconTick");
        }
    }
}