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

namespace VFERomans
{
    public class VFERoman_RepublicFaction : WorldComponent
    {
        //Declare base variables here
        public int nextSenatorID = 1;
        public int nextFactionID = 1;

        public bool spawnTick = true;

        public List<VFERoman_SubFaction> subFactions;


        //Expose data
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<int>(ref nextSenatorID, "nextSenatorID");
            Scribe_Values.Look<int>(ref nextFactionID, "nextFactionID");

            Scribe_Values.Look<bool>(ref spawnTick, "spawnTick");

            Scribe_Collections.Look<VFERoman_SubFaction>(ref subFactions, "subFactions", LookMode.Deep);

        }

        //Constructor
        public VFERoman_RepublicFaction(World world) : base(world)
        {
            var harmony = new Harmony("com.Saakra.VFERomans");
            harmony.PatchAll();
        }





        //World tick
        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            //On first tick of game, prepare the subfactions
            if (this.spawnTick == true)
            {
                subFactions = new List<VFERoman_SubFaction>() { new VFERoman_SubFaction("VFECentralRepublic"), new VFERoman_SubFaction("VFEWesternRepublic"), new VFERoman_SubFaction("VFEEasternImperium") };
               // foreach (VFERoman_SubFaction subfaction in subFactions)
                //{
                    //subfaction.resetResearches();
               // }

                spawnTick = false;

            }

            //SOS2-Compatibility
            //Check if change world, if so run the following:

            /* foreach (VFERoman_SubFaction subfaction in subFactions)
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
