using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEC.Senators
{
    [StaticConstructorOnStartup]
    public class SenatorUIUtility
    {
        public static Texture2D PerkBG_Locked = ContentFinder<Texture2D>.Get("UI/Perks/PerkBG_Locked");
        public static Texture2D PerkBG_United = ContentFinder<Texture2D>.Get("UI/Perks/PerkBG_United");

        static SenatorUIUtility()
        {
            ClassicMod.Harm.Patch(AccessTools.Method(typeof(FactionUIUtility), nameof(FactionUIUtility.DoWindowContents)),
                postfix: new HarmonyMethod(typeof(SenatorUIUtility), nameof(DoPerkButton)));
            ClassicMod.Harm.Patch(AccessTools.Method(typeof(FactionUIUtility), "DrawFactionRow"), new HarmonyMethod(typeof(SenatorUIUtility), nameof(DoSenatorInfoButton)));
        }

        public static void DoPerkButton(Rect fillRect)
        {
            if (Widgets.ButtonText(new Rect(0, 10f, 120f, 30f), "View Perks")) Find.WindowStack.Add(new Dialog_PerkInfo());
        }

        public static void DoSenatorInfoButton(Faction faction, Rect fillRect, float rowY)
        {
            if (faction.def.HasModExtension<FactionExtension_SenatorInfo>())
            {
                fillRect.width -= 130f;
                if (Widgets.ButtonText(new Rect(fillRect.width + 5f, rowY + 25f, 120f, 30f), "View Senators"))
                    Find.WindowStack.Add(new Dialog_SenatorInfo(faction.def.GetModExtension<FactionExtension_SenatorInfo>(), WorldComponent_Senators.Instance.SenatorInfo[faction],
                        false)
                    {
                        Faction = faction
                    });
            }
        }
    }
}