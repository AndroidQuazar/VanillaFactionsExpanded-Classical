using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEC
{
    public class ClassicMod : Mod
    {
        public static Harmony Harm;

        public ClassicMod(ModContentPack content) : base(content)
        {
            Harm = new Harmony("oskarpotocki.vfe.classical");
            Harm.Patch(AccessTools.Method(typeof(Toils_LayDown), "ApplyBedThoughts"), postfix: new HarmonyMethod(GetType(), nameof(TellBedAboutThoughts)));
        }

        public static void TellBedAboutThoughts(Pawn actor)
        {
            if (actor.CurrentBed() is {AllComps: var comps})
                foreach (var comp in comps)
                    comp?.Notify_AddBedThoughts(actor);
        }
    }
}