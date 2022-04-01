using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using VFEC.Perks;

namespace VFEC.Senators
{
    public class Dialog_PerkInfo : Window
    {
        private readonly List<(RepublicDef republic, List<((Faction faction, Texture2D perkBG), List<(PerkDef perk, bool active)>, (PerkDef perk, bool active))> factions, bool
            united)> info;

        public Dialog_PerkInfo()
        {
            info = DefDatabase<RepublicDef>.AllDefs.Select(republicDef => (republicDef, (from factionDef in republicDef.parts
                    let ext = factionDef.GetModExtension<FactionExtension_SenatorInfo>()
                    select ((Find.FactionManager.FirstFactionOfDef(factionDef), ext.PerkBG),
                        ext.senatorPerks.Select(perk => (perk, GameComponent_PerkManager.Instance.ActivePerks.Contains(perk))).ToList(),
                        (ext.finalPerk, GameComponent_PerkManager.Instance.ActivePerks.Contains(ext.finalPerk)))).ToList(),
                GameComponent_PerkManager.Instance.ActivePerks.Contains(republicDef.perk))).ToList();
            doCloseButton = true;
            forcePause = true;
            doCloseX = true;
        }

        public override Vector2 InitialSize => new(1000f, info.Sum(v => v.factions.Count * 200f));

        public override void DoWindowContents(Rect inRect)
        {
            var font = Text.Font;
            var anchor = Text.Anchor;
            var y = inRect.y;
            foreach (var (republic, factions, united) in info)
            {
                var initialHeight = y;
                var x = inRect.x;
                foreach (var ((faction, perkBG), perks, (finalPerk, finalActive)) in factions)
                {
                    x = inRect.x;
                    var rect = new Rect(x, y, 500f, 50f);
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Text.Font = GameFont.Small;
                    Widgets.Label(rect, faction.Name);
                    y += 60f;
                    foreach (var (perk, active) in perks) DoPerkInfo(ref x, y, perk, active, perkBG);

                    Widgets.DrawLine(new Vector2(x, y), new Vector2(x, y + 100f), finalActive ? faction.Color : Color.gray, 3f);
                    x += 15f;
                    DoPerkInfo(ref x, y, finalPerk, finalActive, perkBG);
                    y += 110f;
                }

                var middle = (y - initialHeight) / 2;
                var color = united ? Faction.OfPlayer.Color : Color.gray;
                Widgets.DrawLine(new Vector2(x, initialHeight), new Vector2(x + 20f, middle), color, 3f);
                Widgets.DrawLine(new Vector2(x, y), new Vector2(x + 20f, middle), color, 3f);
                x += 30f;
                DoPerkInfo(ref x, middle - 50f, republic.perk, united, SenatorUIUtility.PerkBG_United);
            }

            Text.Font = font;
            Text.Anchor = anchor;
        }

        private static void DoPerkInfo(ref float x, float y, PerkDef perk, bool active, Texture2D perkBG)
        {
            var rect = new Rect(x, y, 100f, 100f);
            Widgets.DrawTextureFitted(rect, active ? perkBG : SenatorUIUtility.PerkBG_Locked, 1f);
            Widgets.DrawTextureFitted(rect, perk.Icon, 1f);
            TooltipHandler.TipRegion(rect, $"{perk.LabelCap}\n\n{perk.description}");
            x += 110f;
        }
    }
}