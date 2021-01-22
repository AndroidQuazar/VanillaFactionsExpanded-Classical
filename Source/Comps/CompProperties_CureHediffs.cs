using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace VFERomans
{
    public class CompProperties_IgnoreDebuff : CompProperties
    {
        public ThoughtDef removeThought;
        public HediffDef removeHediff;
        public CompProperties_IgnoreDebuff()
        {
            this.compClass = typeof(CompIgnoreDebuff);
        }
    }

    class CompIgnoreDebuff : ThingComp
    {
        public CompProperties_IgnoreDebuff Props
        {
            get
            {
                return (CompProperties_IgnoreDebuff)this.props;
            }
        }

        public override void CompTickRare()
        {
            Building_Bed parentAsBed = (Building_Bed)this.parent;
            if (parentAsBed != null)
            {
                foreach (Pawn pawn in parentAsBed.CurOccupants)
                {
                    // Currently only accepts one Thought or Hediff per comp, make a list of all existing thoughts/hediffs per comp and use a foreach loop to make it applicable multiple times.
                    // Flaw is that once the pawn exits bed the Thought/Hediff returns.
                    // Hediff
                    if (pawn.health.hediffSet.hediffs.Any(h => h.def == this.Props.removeHediff))
                    {
                        pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs.First(h => h.def == this.Props.removeHediff));
                    }
                    // Thought
                    pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(this.Props.removeThought);
                }
            }
        }
    }
}

