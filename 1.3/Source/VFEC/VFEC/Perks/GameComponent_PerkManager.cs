using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEC.Perks
{
    public class GameComponent_PerkManager : GameComponent
    {
        public static GameComponent_PerkManager Instance;
        public HashSet<PerkDef> ActivePerks = new();
        public GameComponent_PerkManager(Game game) => Instance = this;

        public void AddPerk(PerkDef perk)
        {
            ActivePerks.Add(perk);
            perk.Worker.Notify_Added();
        }

        public PerkDef FirstPerk<T>() where T : PerkWorker
        {
            return ActivePerks.FirstOrDefault(perk => perk.Worker is T);
        }

        public void RemovePerk(PerkDef perk)
        {
            ActivePerks.Remove(perk);
            perk.Worker.Notify_Removed();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ActivePerks, "activePerks", LookMode.Def);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                foreach (var perk in ActivePerks)
                    perk.Worker.Notify_Added();
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            foreach (var perk in ActivePerks.ToList()) RemovePerk(perk);
        }
    }
}