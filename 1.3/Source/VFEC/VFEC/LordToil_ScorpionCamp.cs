using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VFEC
{
    public class LordToil_ScorpionCamp : LordToil
    {
        public LordToil_ScorpionCamp(IntVec3 setupSpot, bool useTents)
        {
            data = new LordToilData_ScorpionCamp();
            Data.setupSpot = setupSpot;
            Data.useTents = useTents;
        }

        private LordToilData_ScorpionCamp Data => data as LordToilData_ScorpionCamp;

        public override IntVec3 FlagLoc => Data.setupSpot;

        public override void UpdateAllDuties()
        {
            foreach (var pawn in lord.ownedPawns)
                if (pawn.equipment.Primary.def == VFEC_DefOf.VFEC_Weapon_Scorpion)
                {
                    pawn.mindState.duty = new PawnDuty(VFEC_DefOf.VFEC_ScorpionOperator, Data.setupSpot) {radius = 16f};
                    Data.scorpionOperators.Add(pawn);
                }
                else if (Data.scorpionOperators.Contains(pawn))
                    pawn.mindState.duty = new PawnDuty(VFEC_DefOf.VFEC_ScorpionOperator, Data.setupSpot) {radius = 16f};
                else
                    pawn.mindState.duty = new PawnDuty(DutyDefOf.Defend, Data.setupSpot) {radius = 16f};
        }

        public override void LordToilTick()
        {
            base.LordToilTick();
        }
    }

    // ReSharper disable InconsistentNaming
    public class LordToilData_ScorpionCamp : LordToilData
    {
        public HashSet<Pawn> scorpionOperators = new();
        public IntVec3 setupSpot;
        public bool useTents;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref setupSpot, "setupSpot");
            Scribe_Values.Look(ref useTents, "useTents");
            Scribe_Collections.Look(ref scorpionOperators, "scorpionOperators", LookMode.Reference);
        }
    }
}