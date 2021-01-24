using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URF;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace VFERomans
{
    public class VFERoman_VanillaHarmonyPatches
    {
        [HarmonyPatch(typeof(FactionUIUtility), "DrawFactionRow")]
        class DrawFactionRowPatch
        {
            static void Postfix(Faction faction, float rowY, Rect fillRect)
            {
                if (faction.def.defName == "VFEWesternRepublic" || faction.def.defName == "VFECentralRepublic" || faction.def.defName == "VFEEasternRepublic") {
                    Rect rect = new Rect(320, rowY + 3, 20f, 20f);
                    if (Widgets.ButtonImage(rect, VFERoman_TextureLoader.iconCustomize))
                    {
                        //Open VFERoman_Window_SubFactionInformation
                        Find.WindowStack.Add(new VFERoman_Window_SubFaction(Find.World.GetComponent<VFERoman_RepublicFaction>().subFactions.Where(x => x.faction == faction).First()));
                    }

                    if (Mouse.IsOver(rect))
                    {
                        TipSignal tip = new TipSignal("Open relations menu with " + faction.NameColored);
                        TooltipHandler.TipRegion(rect, tip);
                    }

                    //return false to skip original
                }
            }
        }



        [HarmonyPatch(typeof(Caravan), "GetGizmos")]
        class WorldObjectGizmos
        {
            static void Postfix(ref Caravan __instance, ref IEnumerable<Gizmo> __result)
            {
                VFERoman_RoadBuilder roadBuilder = Find.World.GetComponent<VFERoman_RepublicFaction>().roadBuilder;

                if (__instance.Faction == Find.FactionManager.OfPlayer)
                {
                    int tile = __instance.Tile;
                    string name = __instance.LabelCap;
                    Caravan caravan = __instance;

                    Command_Action actionHostile = new Command_Action
                    {
                        defaultLabel = "VFERBuildRoad".Translate(),
                        defaultDesc = "",
                        icon = null,
                        action = delegate
                        {
                            //Open window to track points
                            //
                            // Window will allow selection or creation of new queues
                            // Can assign a caravan to a queue
                            // Caravan will go to next node to continue work


                            Log.Message("Stop Caravan");
                            caravan.pather.StopDead();
                            //create queue
                        }
                    };







                }
            }
        }

    }
}
