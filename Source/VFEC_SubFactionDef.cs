using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using URF;

namespace VFERomans
{
    public class VFERoman_SubFactionDef : Def, IExposable
    {
        //list vars
        public string name;
        public List<LockedResearchDef> researches;

        public VFERoman_SubFactionDef()
        {

        }

        public void ExposeData()
        {
            //Simple Values
            Scribe_Values.Look<string>(ref name, "name");

            //Collections
            Scribe_Collections.Look<LockedResearchDef>(ref researches, "researches", LookMode.Def);

        }
    }

    [DefOf]
    public class VFERoman_SubFactionDefOf
    {
        public static VFERoman_SubFactionDef VFECentralRepublic;
        public static VFERoman_SubFactionDef VFEWesternRepublic;
        public static VFERoman_SubFactionDef VFEEasternRepublic;
        static VFERoman_SubFactionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(VFERoman_SubFactionDef));
        }
    }

    
}
