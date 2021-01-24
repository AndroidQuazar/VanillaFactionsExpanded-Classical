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
    public class VFEC_Senator : ILoadReferenceable, IExposable
    {
        public int loadID;
        public Pawn pawn;
        public VFEC_SubFaction subFaction;
        public int opinion;
        public bool isEmperor;
        public bool isSupporting;

        public VFEC_Senator()
        {

        }
        public VFEC_Senator( VFEC_SubFaction subFaction, bool isEmperor)
        {
            this.subFaction = subFaction;
            generateNewSenator(isEmperor);
            SetUniqueLoadID();
            this.opinion = 0;
            this.isSupporting = false;
        }


        public void generateNewSenator(bool isEmperor = false)
        {
            //Add non-violent traits
            Pawn newPawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Empire_Royal_Bestower, this.subFaction.faction));
            if (newPawn != null)
            {
                this.pawn = newPawn;
                //Log.Message(this.pawn.Name + " generated as a senator for " + this.subFaction.def.name);
            }
            else
            {
                Log.Error("VFFERoman_Senator - Pawn created returned null");
            }
            this.isEmperor = isEmperor;
        }

        public void addSupport()
        {
            this.isSupporting = true;
            this.subFaction.updateSenateSupport();
        }

        //Referenceable
        public string GetUniqueLoadID()
        {
            return "VFEC_Senator_" + this.loadID;
        }
        public void SetUniqueLoadID()
        {
            this.loadID = Find.World.GetComponent<VFEC_RepublicFaction>().getNextSenatorID();
        }

        //Expose Data
        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref opinion, "opinion");
            Scribe_Values.Look<int>(ref loadID, "loadID");
            Scribe_Values.Look<bool>(ref isEmperor, "isEmperor");
            Scribe_Values.Look<bool>(ref isSupporting, "isSupporting");
            Scribe_Deep.Look<Pawn>(ref pawn, "pawn");
            Scribe_References.Look<VFEC_SubFaction>(ref subFaction, "subFaction");
        }


    }
}
