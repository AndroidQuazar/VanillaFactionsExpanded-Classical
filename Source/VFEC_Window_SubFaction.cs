using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;


namespace VFEC
{
    class VFEC_Window_SubFaction : Window
    {
		float basePawnWidth = 100;
		float basePawnHeight = 100;
		float pawnDistance = 90;
		float pawnDistanceSize = .88f;
		float pawnDistanceHeight = -50f;

		VFEC_SubFaction subfaction;

		public VFEC_Window_SubFaction(VFEC_SubFaction subFaction)
		{
			if (subFaction == null)
			{
				this.Close();
			}
			this.subfaction = subFaction;
			this.forcePause = false;
			this.draggable = true;
			this.doCloseX = true;
			this.preventCameraMotion = false;
		}

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(838f, 538f);
			}
		}

		public override void PreOpen()
		{
			base.PreOpen();
			subfaction.removeDeadSenatorsOrCreateNew();
			Log.Message("Number of senators: " + subfaction.senators.Count());
		}

		public override void DoWindowContents(Rect inRect)
		{
			//Start function
			GameFont fontBefore = Text.Font;
			TextAnchor anchorBefore = Text.Anchor;

			//Declare rects
			Rect rectBasePawn = new Rect(InitialSize.x / 2 - basePawnWidth / 2, InitialSize.y / 2 - basePawnHeight / 2, basePawnWidth, basePawnWidth);
			Rect rectbasePawnCircle = new Rect(rectBasePawn.x, (float)(rectBasePawn.y + rectBasePawn.height*.8), basePawnWidth, basePawnWidth/2);
			Rect name = new Rect(rectBasePawn.x, rectBasePawn.y - (float)(rectBasePawn.height*.6), basePawnWidth, 60);
			Rect opinion = new Rect(rectBasePawn.x, (float)(rectBasePawn.y + rectBasePawn.height*1.3), basePawnWidth, 30);

			Rect factionName = new Rect(0, 5, InitialSize.x, 150);
			Rect leftButton = new Rect(10, InitialSize.y - 60 - 38, 180, 50);
			Rect rightButton = new Rect(InitialSize.x - 10 - 180 - 38, InitialSize.y - 60 - 38, 180, 50);



			//iterate(int i, pawn distance size, pawn distance height)

			
			for (int i = 0; i < subfaction.senators.Count(); i++)
			{
				double center = i - 3;
				double sizeModifier = Math.Pow(pawnDistanceSize, Math.Abs(center));

				float newX = (float)center * pawnDistance;
				float newY = iterate(center);//Math.Abs((float)center * pawnDistanceHeight);
				float newWidthMod = (float)(sizeModifier);
				float newHeightMod = (float)(sizeModifier);

				if (center < 0)
				{
					newX += (float)(rectBasePawn.width * (1-sizeModifier));
				}
				float labelAdjust = (float)(rectBasePawn.height * (1-sizeModifier));

				Rect newRect = new Rect(rectBasePawn.x + newX,rectBasePawn.y - newY, rectBasePawn.width * newWidthMod, rectBasePawn.height * newHeightMod);
				Rect newRectCircle = new Rect(rectbasePawnCircle.x + newX, rectbasePawnCircle.y - newY - labelAdjust, rectbasePawnCircle.width * newWidthMod, rectbasePawnCircle.height * newHeightMod);
				Rect newRectName = new Rect(name.x + newX - (float)(name.width*(1-sizeModifier)/2), name.y - newY, name.width, name.height);
				Rect newRectOpinion = new Rect(opinion.x + newX - (float)(opinion.width*(1-sizeModifier)/2), opinion.y - newY - labelAdjust - (float)(opinion.height*(1-sizeModifier)), opinion.width, opinion.height);

				int k = i;
				if (Widgets.ButtonInvisible(newRect))
				{
					List<FloatMenuOption> list = new List<FloatMenuOption>();

					list.Add(new FloatMenuOption("View Quests", delegate
					{

					}));

					list.Add(new FloatMenuOption("Send Gift", delegate
					{

					}));

					//if (subfaction.senators[k].isSupporting == false) { }
					list.Add(new FloatMenuOption("Request Support", delegate
					{
						subfaction.senators[k].opinion = 100;
						subfaction.senators[k].addSupport();
					}));

					FloatMenu menu = new FloatMenu(list);
					Find.WindowStack.Add(menu);

				}

				circleUnderPawn(newRectCircle);
				Widgets.ThingIcon(newRect, subfaction.senators[i].pawn);
				
				

				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.LowerCenter;
				Widgets.Label(newRectName, subfaction.senators[i].pawn.NameFullColored);
				Text.Anchor = TextAnchor.UpperCenter;
				Widgets.Label(newRectOpinion, subfaction.senators[i].opinion.ToString());

				Text.Font = GameFont.Medium;
				Widgets.Label(factionName, subfaction.faction.Name);

				if(Widgets.ButtonText(leftButton, "View Researches"))
				{

				}

				if (Widgets.ButtonText(rightButton, "Request Tribute"))
				{

				}

			}









			//End Function
			Text.Font = fontBefore;
			Text.Anchor = anchorBefore;
		}

		public float iterate(double center)
		{
			float k = (float)Math.Abs(center);
			float heightOffset = 0;
			for (int i = 0; i <= k; i++)
			{
				float sizeModifier = (float)Math.Pow(pawnDistanceSize, k);
				heightOffset -= (pawnDistanceHeight * sizeModifier);
			}
			return heightOffset;
		}

		public Rect adjustRect(Rect rect, float x, float y, float width = 1f, float height = 1f, bool scaleWidth = false, float overrideWidth = 0f, float overrideHeight = 0f) 
		{
			Rect temp = new Rect();
			temp = rect;
			if (x < 0)
			{
				x -= ((1-width)*pawnDistanceSize)/2;
			}

			temp.x += x;
			temp.y += y;
			temp.width *= width;
			temp.height *= height;

			if (scaleWidth)
			{
				if (x < 0)
				{
					//temp.x -= temp.width;
				}
			}

			if (overrideWidth != 0)
			{
				temp.width = overrideWidth;
			}
			if (overrideHeight != 0)
			{
				temp.width = overrideHeight;
			}

			return temp;
		}

		public void circleUnderPawn(Rect pawnRectCircle)
		{
			Widgets.ButtonImage(pawnRectCircle, VFEC_TextureLoader.pawnCircle);
		}
	}
}
