using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URF;
using RimWorld;
using RimWorld.Planet;
using Verse;
using HarmonyLib;

namespace VFEC
{
    public class VFEC_RepublicFaction : WorldComponent
    {
        //Declare base variables here
        public int nextSenatorID = 1;
        public int nextFactionID = 1;

        public bool spawnTick = true;

        public List<VFEC_SubFaction> subFactions;


        //Expose data
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<int>(ref nextSenatorID, "nextSenatorID");
            Scribe_Values.Look<int>(ref nextFactionID, "nextFactionID");

            Scribe_Values.Look<bool>(ref spawnTick, "spawnTick");

            Scribe_Collections.Look<VFEC_SubFaction>(ref subFactions, "subFactions", LookMode.Deep);

        }

        //Constructor
        public VFEC_RepublicFaction(World world) : base(world)
        {
            var harmony = new Harmony("com.Saakra.VFEC");
            harmony.PatchAll();
        }





        //World tick
        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            //On first tick of game, prepare the subfactions
            if (this.spawnTick == true)
            {
                subFactions = new List<VFEC_SubFaction>() { new VFEC_SubFaction("VFECentralRepublic"), new VFEC_SubFaction("VFEWesternRepublic"), new VFEC_SubFaction("VFEEasternImperium") };
               // foreach (VFEC_SubFaction subfaction in subFactions)
                //{
                    //subfaction.resetResearches();
               // }

                spawnTick = false;

            }

            //SOS2-Compatibility
            //Check if change world, if so run the following:

            /* foreach (VFEC_SubFaction subfaction in subFactions)
            {
                subfaction.updateFaction();
            }
            */


            //Start World ticking here


        }


        //IloadReferenable Counters
        public int getNextSenatorID()
        {
            this.nextSenatorID++;
            return nextSenatorID;
        }
        public int getNextFactionID()
        {
            this.nextFactionID++;
            return nextFactionID;
        }

    }
}
