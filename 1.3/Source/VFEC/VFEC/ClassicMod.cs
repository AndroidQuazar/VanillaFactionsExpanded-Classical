using HarmonyLib;
using Verse;

namespace VFEC
{
    public class ClassicMod : Mod
    {
        public static Harmony Harm;

        public ClassicMod(ModContentPack content) : base(content) => Harm = new Harmony("oskarpotocki.vfe.classical");
    }
}