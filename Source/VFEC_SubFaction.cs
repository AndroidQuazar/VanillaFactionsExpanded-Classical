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
    public class VFEC_SubFaction : ILoadReferenceable, IExposable
    {
        //Init Vars
        public int loadID;
        public int senatorSupportReached = 0;

        public VFEC_SubFactionDef def;
        public Faction faction;
        public List<VFEC_Senator> senators = new List<VFEC_Senator>();



        public VFEC_SubFaction()
        {

        }

        //Creation
        public VFEC_SubFaction(string defName)
        {
            SetUniqueLoadID();
            //load def and find faction
            switch (defName)
            {
                
                case "VFECentralRepublic":
                    def = VFEC_SubFactionDefOf.VFECentralRepublic;
                    break;
                case "VFEWesternRepublic":
                    def = VFEC_SubFactionDefOf.VFEWesternRepublic;
                    break;
                case "VFEEasternRepublic":
                    def = VFEC_SubFactionDefOf.VFEEasternRepublic;
                    break;

            }
           // Log.Message("Loading VFE Roman Subfaction - " + def.name);
            updateFaction();
            removeDeadSenatorsOrCreateNew();

        }


        //Reset Researches - Use on Creation
        public void resetResearches()
        {

            //Log.Message("Displaying Researches in Util...");
            foreach (LockedResearch research in Find.World.GetComponent<ResearchDiscovery>().lockedResearches)
            {
                Log.Message(research.def.defName);
            }
        }

        public bool unlockTech(string key)
        {
            fixTechs();
            ResearchDiscovery researchDiscovery = Find.World.GetComponent<ResearchDiscovery>();


            bool value = researchDiscovery.sendUnlockCode(key);
            researchDiscovery.updateLockedResearches();

            return value;
        }

        public void fixTechs()
        {
            ResearchDiscovery researchDiscovery = Find.World.GetComponent<ResearchDiscovery>();
            foreach (LockedResearch research in researchDiscovery.lockedResearches)
            {
                research.classMethodToRun = research.def.classMethodToRunUponUnlocking;
                research.classToRun = research.def.classToRunUponUnlocking;
                research.passResearchLockToMethod = research.def.passResearchLockToMethod;
            }
        }

        //Used to find factions (Especially when switching planets)
        public void updateFaction()
        {
            Log.Message(faction.def.defName);
            Log.Message(def.defName);
            this.faction = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed(def.defName));
            if (this.faction != null)
            {
               // Log.Message("Updated faction to " + this.faction.Name);
            } else
            {
               // Log.Error("Could not find existing faction of VFE faction");
            }
        }

        //Create senators
        public void removeDeadSenatorsOrCreateNew()
        {
            bool isEmperor;
            //for senators 1-6
            for (int i = 0; i < 7; i++)
            {
                if (i == 3)
                {
                    isEmperor = true;
                } else
                {
                    isEmperor = false;
                }
                if (senators.Count < i + 1)
                {
                    //if pawn does not exist

                    senators.Add(new VFEC_Senator(this, isEmperor));

                } 
                
                else if (senators[i] != null && senators[i].pawn != null && senators[i].pawn.Dead)
                {
                    //if pawn is dead

                    //pass pawn to world dead
                    if (!Find.WorldPawns.Contains(senators[i].pawn))
                    {
                        Find.WorldPawns.PassToWorld(senators[i].pawn, PawnDiscardDecideMode.KeepForever);
                    }

                    senators[i] = new VFEC_Senator(this, isEmperor);

                } 
                else
                {
                    //if pawn exists and is not dead

                }
                
            }
           // Log.Message("Setting faction leader");
            //set senator 3 as faction leader
            faction.leader = senators[3].pawn;
        }

        public void unlockNextTechnology()
        {
            string str = "VFER_";
            switch (this.def.defName)
            {
                case "VFECentralRepublic":
                    str = str + "CR_";
                    break;
                case "VFEWesternRepublic":
                    str = str + "WR_";
                    break;
                case "VFEEasternRepublic":
                    str = str + "EI_";
                    break;
            }
            str = str + this.senatorSupportReached;
           // Log.Message(str);
            this.unlockTech(str);
        }

        public int updateSenateSupport()
        {
            int tally = 0;
            foreach (VFEC_Senator senator in senators)
            {
                if (senator.isSupporting)
                    tally += 1;
            }
            if (this.senatorSupportReached < tally)
            {
                this.senatorSupportReached = tally;
                this.unlockNextTechnology();
            }
            return tally;
        }

        //Referenceable
        public string GetUniqueLoadID()
        {
            return "VFERoman_SubFaction_" + this.loadID;
        }
        public void SetUniqueLoadID()
        {
            this.loadID = Find.World.GetComponent<VFEC_RepublicFaction>().getNextFactionID();
        }

        //Expose data
        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref loadID, "loadID");
            Scribe_Values.Look<int>(ref senatorSupportReached, "senatorSupportReached");
            Scribe_Defs.Look<VFEC_SubFactionDef>(ref def, "def");
            Scribe_Collections.Look<VFEC_Senator>(ref senators, "senators", LookMode.Deep);
            Scribe_References.Look<Faction>(ref faction, "faction");
        }
    }
}
