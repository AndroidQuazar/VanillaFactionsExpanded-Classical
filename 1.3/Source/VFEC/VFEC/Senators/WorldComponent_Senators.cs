using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VFEC.Senators
{
    [StaticConstructorOnStartup]
    public class WorldComponent_Senators : WorldComponent
    {
        public static WorldComponent_Senators Instance;

        private List<FactionInfos> infos = new();
        public int NumBribes;

        public Dictionary<Faction, List<SenatorInfo>> SenatorInfo = new();

        static WorldComponent_Senators()
        {
            ClassicMod.Harm.Patch(AccessTools.Method(typeof(Settlement), nameof(Settlement.GetFloatMenuOptions)),
                postfix: new HarmonyMethod(typeof(WorldComponent_Senators), nameof(AddSenatorsOption)));
        }

        public WorldComponent_Senators(World world) : base(world) => Instance = this;

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            if (Current.CreatingWorld is null) return;
            foreach (var faction in world.factionManager.AllFactions)
                if (faction.def.HasModExtension<FactionExtension_SenatorInfo>())
                    SenatorInfo.Add(faction, Enumerable.Repeat((false, true), faction.def.GetModExtension<FactionExtension_SenatorInfo>().numSenators).Select(info =>
                        new SenatorInfo
                        {
                            Pawn = GenerateSenator(faction),
                            Favored = info.Item1,
                            CanBribe = info.Item2,
                            Quest = null
                        }).ToList());
        }

        public Pawn GenerateSenator(Faction faction)
        {
            var pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest
                {KindDef = VFEC_DefOf.VFEC_RepublicSenator, Faction = faction, AllowDead = false, ForceGenerateNewPawn = true, Context = PawnGenerationContext.NonPlayer});
            world.worldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
            return pawn;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref NumBribes, "numBribes");
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                infos.Clear();
                infos.AddRange(SenatorInfo.Select(kv => new FactionInfos
                {
                    Faction = kv.Key,
                    Infos = kv.Value
                }));
            }

            Scribe_Collections.Look(ref infos, "infos", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) SenatorInfo = infos.ToDictionary(info => info.Faction, info => info.Infos);
        }

        public static IEnumerable<FloatMenuOption> AddSenatorsOption(IEnumerable<FloatMenuOption> options, Settlement __instance, Caravan caravan) =>
            __instance.Faction.def.HasModExtension<FactionExtension_SenatorInfo>() && __instance.Tile == caravan.Tile
                ? options.Append(new FloatMenuOption("VFEC.Senators.Open".Translate(), () =>
                    Find.WindowStack.Add(new Dialog_SenatorInfo(__instance.Faction.def.GetModExtension<FactionExtension_SenatorInfo>(), Instance.SenatorInfo[__instance.Faction])
                    {
                        Caravan = caravan,
                        Faction = __instance.Faction
                    })))
                : options;

        public void GainFavorOf(Pawn pawn, Faction faction)
        {
            var info = InfoFor(pawn, faction);
            info.Favored = true;
            // TODO: Senator joins you, and get a letter
        }

        public SenatorInfo InfoFor(Pawn pawn, Faction faction)
        {
            return SenatorInfo[faction].Find(i => i.Pawn == pawn);
        }

        private class FactionInfos : IExposable
        {
            public Faction Faction;
            public List<SenatorInfo> Infos;

            public void ExposeData()
            {
                Scribe_References.Look(ref Faction, "faction");
                Scribe_Collections.Look(ref Infos, "infos", LookMode.Deep);
            }
        }
    }

    public class SenatorInfo : IExposable
    {
        public bool CanBribe;
        public bool Favored;
        public Pawn Pawn;
        public Quest Quest;

        public void ExposeData()
        {
            Scribe_References.Look(ref Pawn, "pawn");
            Scribe_References.Look(ref Quest, "quest");
            Scribe_Values.Look(ref Favored, "favored");
            Scribe_Values.Look(ref CanBribe, "canBribe");
        }
    }
}