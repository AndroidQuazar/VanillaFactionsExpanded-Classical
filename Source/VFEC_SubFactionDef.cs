using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using URF;

namespace VFEC
{
    public class VFEC_SubFactionDef : Def, IExposable
    {
        //list vars
        public string name;
        public List<LockedResearchDef> researches;

        public VFEC_SubFactionDef()
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
    public class VFEC_SubFactionDefOf
    {
        public static VFEC_SubFactionDef VFECentralRepublic;
        public static VFEC_SubFactionDef VFEWesternRepublic;
        public static VFEC_SubFactionDef VFEEasternRepublic;
        static VFEC_SubFactionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(VFEC_SubFactionDefOf));
        }
    }

    
}
