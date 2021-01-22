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

namespace VFEC
{
    public class VFEC_VanillaHarmonyPatches
    {
        [HarmonyPatch(typeof(FactionUIUtility), "DrawFactionRow")]
        class DrawFactionRowPatch
        {
            static void Postfix(Faction faction, float rowY, Rect fillRect)
            {
                if (faction.def.defName == "VFECWesternRepublic" || faction.def.defName == "VFECCentralRepublic" || faction.def.defName == "VFECEasternImperium") {
                    Rect rect = new Rect(320, rowY + 3, 20f, 20f);
                    if (Widgets.ButtonImage(rect, VFEC_TextureLoader.iconCustomize))
                    {
                        //Open VFEC_Window_SubFactionInformation
                        Find.WindowStack.Add(new VFEC_Window_SubFaction(Find.World.GetComponent<VFEC_RepublicFaction>().subFactions.Where(x => x.faction == faction).First()));
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

    }
}
