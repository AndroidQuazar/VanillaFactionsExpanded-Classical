using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using VFEC.Perks;
using VFECore.UItils;

namespace VFEC.Senators
{
    [StaticConstructorOnStartup]
    public class Dialog_SenatorInfo : Window
    {
        public static Texture2D PerkBG_Locked = ContentFinder<Texture2D>.Get("UI/Perks/PerkBG_Locked");

        private static readonly Color DisplayBGColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 15);
        private readonly PerkDef finalPerk;
        private readonly ResearchProjectDef finalResearch;
        private readonly Texture2D perkBG_Unlocked;
        private readonly List<AllSenatorInfo> senatorInfo;
        public Caravan Caravan;
        public Faction Faction;

        private RenderTexture pawnTexture;

        public Dialog_SenatorInfo(FactionExtension_SenatorInfo senatorInfo1, List<SenatorInfo> senatorInfo2)
        {
            senatorInfo = new List<AllSenatorInfo>();
            finalPerk = senatorInfo1.finalPerk;
            finalResearch = senatorInfo1.finalResearch;
            perkBG_Unlocked = senatorInfo1.PerkBG;
            Log.Message($"Found {senatorInfo1.numSenators} senators");
            Log.Message("Perks:");
            GenDebug.LogList(senatorInfo1.senatorPerks);
            Log.Message("Researches:");
            GenDebug.LogList(senatorInfo1.senatorResearch);
            Log.Message("Infos:");
            foreach (var info in senatorInfo2) Log.Message($"  pawn={info.Pawn},favor={info.Favor},canBribe={info.CanBribe},quest={info.Quest}");
            for (var i = 0; i < senatorInfo1.numSenators; i++)
                senatorInfo.Add(AllSenatorInfo.FromSenatorInfo(senatorInfo2[i], senatorInfo1.senatorPerks[i], senatorInfo1.senatorResearch[i]));
            doCloseButton = false;
            doCloseX = false;
            forcePause = true;
        }

        protected override float Margin => 12f;
        public override Vector2 InitialSize => new(1150, 750);

        private Texture2D PerkBG(bool locked) => locked ? PerkBG_Locked : perkBG_Unlocked;

        public override void DoWindowContents(Rect inRect)
        {
            var font = Text.Font;
            var anchor = Text.Anchor;
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(inRect.TakeTopPart(40f), "VFEC.UI.SenatorsOf".Translate(Faction.Name));
            foreach (var (info, rect) in senatorInfo.Zip(UIUtility.Divide(inRect.TakeTopPart(550f), senatorInfo.Count, senatorInfo.Count, 1, false),
                (info, rect) => (info, rect.ContractedBy(7f, 0))))
                DrawSenatorInfo(info, rect);

            Text.Anchor = TextAnchor.UpperLeft;
            var allInfo = inRect.TakeLeftPart(250f);
            Text.Font = GameFont.Small;
            Widgets.Label(allInfo.TakeTopPart(40f), "VFEC.UI.GainingFavorAll".Translate());
            Text.Anchor = TextAnchor.MiddleLeft;
            DoRewardsInfo(allInfo.TakeTopPart(40f), finalPerk, finalResearch);

            DoPerkInfo(inRect.TakeLeftPart(100f), finalPerk, true);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            var textRect = inRect.TakeLeftPart(500f);
            Widgets.Label(textRect.TakeTopPart(20f), "VFEC.UI.SenatorsInFavor".Translate(0, senatorInfo.Count));
            Widgets.Label(textRect, "");

            if (Widgets.ButtonText(inRect.TakeBottomPart(40f).ContractedBy(10f, 0), "Close".Translate())) Close();

            Text.Font = font;
            Text.Anchor = anchor;
        }

        private void DrawSenatorInfo(AllSenatorInfo info, Rect inRect)
        {
            var portraitRect = inRect.TakeTopPart(200f);
            Widgets.DrawBoxSolid(portraitRect, DisplayBGColor);
            portraitRect = portraitRect.ContractedBy(3f);
            var pawnTexture = GetPawnTexture(info.Pawn, portraitRect.size, Rot4.South);
            GUI.DrawTexture(portraitRect, pawnTexture);

            Text.Font = GameFont.Medium;
            Widgets.Label(inRect.TakeTopPart(30f), info.Pawn.Name.ToStringFull);

            Text.Font = GameFont.Tiny;
            Widgets.Label(inRect.TakeTopPart(20f), "PawnMainDescFactionedWrap".Translate(VFEC_DefOf.VFEC_RepublicSenator.LabelCap, Faction.Name));

            Widgets.DrawLineHorizontal(inRect.x, inRect.y, inRect.width);
            inRect.yMin += 7f;

            Text.Font = GameFont.Small;
            Widgets.ButtonText(inRect.TakeTopPart(40f).ContractedBy(10f, 0f), "VFEC.UI.ReQuest".Translate());
            if (info.CanBribe) Widgets.ButtonText(inRect.TakeTopPart(40f).ContractedBy(10f, 0f), "VFEC.UI.Bribe".Translate(1000));
            else inRect.yMin += 40f;

            Text.Font = GameFont.Small;
            Widgets.Label(inRect.TakeTopPart(40f), "VFEC.UI.GainingFavor".Translate());

            DoRewardsInfo(inRect.TakeTopPart(50f), info.Perk, info.Research);

            DoPerkInfo(inRect.TakeTopPart(100f).ContractedBy((inRect.width - 100f) / 2, 0f), info.Perk, true);
        }

        private void DoPerkInfo(Rect inRect, PerkDef perk, bool locked)
        {
            Widgets.DrawTextureFitted(inRect, PerkBG(locked), 1f);
            Widgets.DrawTextureFitted(inRect.ContractedBy(10f), perk.Icon, 1f);
            TooltipHandler.TipRegion(inRect, $"{perk.LabelCap}\n\n{perk.description}");
        }

        private void DoRewardsInfo(Rect inRect, PerkDef perk, ResearchProjectDef research)
        {
            Text.Font = GameFont.Tiny;
            Widgets.Label(inRect.TopHalf(), "VFEC.UI.Perk".Translate(perk.LabelCap).Colorize(ColoredText.SubtleGrayColor));
            Widgets.Label(inRect.BottomHalf(), "VFEC.UI.Research".Translate(research.LabelCap).Colorize(ColoredText.SubtleGrayColor));
        }

        public RenderTexture GetPawnTexture(Pawn pawn, Vector2 size, Rot4 rotation, Vector3 cameraOffset = default, float cameraZoom = 1f, bool supersample = true,
            bool compensateForUIScale = true, bool renderHeadgear = true, bool renderClothes = true, Dictionary<Apparel, Color> overrideApparelColors = null,
            Color? overrideHairColor = null, bool stylingStation = false)
        {
            if (supersample) size *= 1.25f;

            if (compensateForUIScale) size *= Prefs.UIScale;

            var angle = 0f;
            var positionOffset = default(Vector3);
            if (pawn.Dead || pawn.Downed)
            {
                angle = 85f;
                positionOffset.x -= 0.18f;
                positionOffset.z -= 0.18f;
            }

            var renderTexture = NewRenderTexture(size);
            Find.PawnCacheRenderer.RenderPawn(pawn, renderTexture, cameraOffset, cameraZoom, angle, rotation, pawn.health.hediffSet.HasHead,
                true, renderHeadgear, renderClothes, false, positionOffset, overrideApparelColors, overrideHairColor, stylingStation);
            return renderTexture;
        }

        private RenderTexture NewRenderTexture(Vector2 size)
        {
            if (pawnTexture is null)
                pawnTexture = new RenderTexture((int) size.x, (int) size.y, 24)
                {
                    name = "Portrait",
                    useMipMap = false,
                    filterMode = FilterMode.Bilinear
                };

            return pawnTexture;
        }

        private class AllSenatorInfo : SenatorInfo
        {
            public PerkDef Perk;
            public ResearchProjectDef Research;

            public static AllSenatorInfo FromSenatorInfo(SenatorInfo info, PerkDef perk, ResearchProjectDef research) =>
                new()
                {
                    Pawn = info.Pawn,
                    Favor = info.Favor,
                    CanBribe = info.CanBribe,
                    Quest = info.Quest,
                    Perk = perk,
                    Research = research
                };
        }
    }
}