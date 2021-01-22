using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace VFEC
{
    public class CompProperties_IgnoreDebuffs : CompProperties
    {
        public ThoughtDef removeThought;
        public HediffDef removeHediff;
        public List<ThoughtDef> ThoughtsToRemove = new List<ThoughtDef>();
        public List<HediffDef> HediffsToRemove = new List<HediffDef>();

        public CompProperties_IgnoreDebuffs()
        {
            this.compClass = typeof(CompIgnoreDebuff);
        }
    }

    class CompIgnoreDebuff : ThingComp
    {
        public CompProperties_IgnoreDebuffs Props
        {
            get
            {
                return (CompProperties_IgnoreDebuffs)this.props;
            }
        }

        public override void CompTickRare()
        {
            Building_Bed parentAsBed = (Building_Bed)this.parent;
            if (parentAsBed != null)
            {
                foreach (Pawn pawn in parentAsBed.CurOccupants)
                {
                    // Current flaw is that once the pawn exits bed the Thought/Hediff returns.

                    // Hediff
                    foreach (HediffDef hediffdef in this.Props.HediffsToRemove)
                    {
                        if (pawn.health.hediffSet.hediffs.Any(h => h.def == this.Props.removeHediff))
                        {
                            pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs.First(h => h.def == this.Props.removeHediff));
                        }
                    }
                    // Thought
                    foreach (ThoughtDef thoughtdef in this.Props.ThoughtsToRemove)
                    {
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(this.Props.removeThought);
                    }
                }
            }
        }
    }
}

