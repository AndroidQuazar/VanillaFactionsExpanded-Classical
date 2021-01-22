using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;

namespace VFEC
{
    class VFEC_WorkGiver_DryMeat : WorkGiver_Scanner
    {

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(DefDatabase<ThingDef>.GetNamed("VFE_MeatDryingRack"));
            }
        }


        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.ClosestTouch;
            }
        }

        public VFEC_WorkGiver_DryMeat()
        {
            //if (VFEC_WorkGiver_DryMeat.No)
        }


        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return base.JobOnThing(pawn, t, forced);
        }

    }
}
