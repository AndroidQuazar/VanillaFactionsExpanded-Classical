using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;

namespace VFERomans
{
	class VFERoman_Window_RoadQueues : Window
	{ 
		public int scroll = 0;
		public int maxScroll;
		public int scrollBoxHeight = 200;

		public int yOffset = 0;
		public int buttonWidth = 142;
		public int buttonHeight = 40;
		public int buttonXOffset = 5;
		public int scrollFieldHeight = 40;
		public VFERoman_RoadBuilder roadBuilder;
		public VFERoman_RoadQueue queue = null;
		public string inputText = "New Queue";

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(338f, 538f);
			}
		}

		public VFERoman_Window_RoadQueues()
		{
			//Window Information
			this.roadBuilder = Find.World.GetComponent<VFERoman_RepublicFaction>().roadBuilder;
			this.maxScroll = (roadBuilder.queues.Count() * scrollBoxHeight) - scrollBoxHeight;

			//Window Properties
			this.forcePause = false;
			this.draggable = true;
			this.doCloseX = true;
			this.preventCameraMotion = false;
		}

		public override void PreOpen()
		{
			base.PreOpen();
		}

		public override void DoWindowContents(Rect inRect)
		{
			//Start function
			GameFont fontBefore = Text.Font;
			TextAnchor anchorBefore = Text.Anchor;

			//Declare Rects
			Rect input = new Rect(0, yOffset + 0, 300, 50);
			Rect inputName = new Rect(0, yOffset + 0, 250, 45);
			Rect inputNameButton = new Rect(250, yOffset + 0, 50, 45);
			
			Rect selectionBox = new Rect(0, yOffset + 50, 300, scrollBoxHeight);
			int tempY = yOffset + 50 + scrollBoxHeight;
			Rect restOfTheDamnMenu = new Rect(0, tempY, 300, InitialSize.y - tempY);

			Rect button_ViewQueue = new Rect(buttonXOffset, restOfTheDamnMenu.y, buttonWidth, buttonHeight);
			Rect button_AssignQueue = new Rect(buttonXOffset, restOfTheDamnMenu.y + buttonHeight + 10, buttonWidth, buttonHeight);
			Rect button_RenameQueue = new Rect(300 - buttonWidth - buttonXOffset, restOfTheDamnMenu.y, buttonWidth, buttonHeight);
			Rect button_DeleteQueue = new Rect(300 - buttonWidth - buttonXOffset, restOfTheDamnMenu.y + buttonHeight + 10, buttonWidth, buttonHeight);


			if (Widgets.ButtonInvisible(inputNameButton))
			{
				//create new queue
				Log.Message("Debug - Created new queue");
				roadBuilder.buildNewQueue();

			}

			Widgets.DrawHighlight(selectionBox);

			int i = 0;
			foreach (VFERoman_RoadQueue selectedQueue in roadBuilder.queues)
			{
				VFERoman_RoadQueue tempq = selectedQueue;
				Rect tempRect = new Rect(selectionBox.x, yOffset + 50 + scrollFieldHeight * i - scroll, 300, scrollFieldHeight);
				Widgets.DrawMenuSection(tempRect);
				if (tempq == queue)
					Widgets.DrawLightHighlight(tempRect);
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(tempRect, tempq.queueName);
				if (Widgets.ButtonInvisible(tempRect) && Mouse.IsOver(input) == false && Mouse.IsOver(restOfTheDamnMenu) == false)
				{
					queue = tempq;
				}
				i++;
			}


			Widgets.DrawMenuSection(restOfTheDamnMenu);
			//View Queue
			if (Widgets.ButtonTextSubtle(button_ViewQueue, "VFERViewQueue".Translate()))
			{
				if (queue == null)
				{
					Messages.Message("Debug - No queue selected", MessageTypeDefOf.NeutralEvent);
				} else
				{
					//If queue is selected

				}
			}
			//Rename Queue
			if (Widgets.ButtonTextSubtle(button_RenameQueue, "VFERRenameQueue".Translate()))
			{
				if (queue == null)
				{
					Messages.Message("Debug - No queue selected", MessageTypeDefOf.NeutralEvent);
				}
				else
				{
					//If queue is selected

				}
			}
			//Assign Queue
			if (Widgets.ButtonTextSubtle(button_AssignQueue, "VFERAssignQueue".Translate()))
			{
				if (queue == null)
				{
					Messages.Message("Debug - No queue selected", MessageTypeDefOf.NeutralEvent);
				}
				else
				{
					//If queue is selected

				}
			}
			//Delete Queue
			if (Widgets.ButtonTextSubtle(button_DeleteQueue, "VFERDeleteQueue".Translate()))
			{
				if (queue == null)
				{
					Messages.Message("Debug - No queue selected", MessageTypeDefOf.NeutralEvent);
				}
				else
				{
					//If queue is selected

				}
			}

			Widgets.DrawMenuSection(input);
			string inputString = Widgets.TextField(inputName, inputText);
			inputText = inputString;

			Widgets.ButtonTextSubtle(inputNameButton, "");
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inputNameButton, ">");



			//End Function
			Text.Font = fontBefore;
			Text.Anchor = anchorBefore;

			if (Event.current.type == EventType.ScrollWheel)
			{
				scrollWindow(Event.current.delta.y);
			}
		}

		private void scrollWindow(float num)
		{
			if (scroll - num * 5 < -1 * maxScroll)
			{
				scroll = -1 * maxScroll;
			}
			else if (scroll - num * 5 > 0)
			{
				scroll = 0;
			}
			else
			{
				scroll -= (int)Event.current.delta.y * 5;
			}
			Event.current.Use();
		}




	}
}
