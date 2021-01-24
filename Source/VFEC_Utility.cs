using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URF;
using RimWorld;
using RimWorld.Planet;
using Verse;
using HarmonyLib;


namespace VFEC
{
	public class VFECs_Utility
	{

		public VFECs_Utility()
		{

		}

		//Checks if mod is being used
		public static bool checkForMod(string packageID)
		{
			foreach (ModContentPack mod in LoadedModManager.RunningModsListForReading)
			{
				//Log.Message(mod.PackageIdPlayerFacing);
				if (mod.PackageIdPlayerFacing == packageID)
				{
					return true;
				}
			}

			return false;
		}

		
		//Test Log
		public static void LogSuccess()
		{
		}

		public static void AutoCompleteTechnology(LockedResearch locked)
		{
			foreach (ResearchProjectDef def in locked.lockedResearches)
			{
				Log.Message(def.defName);
				Find.ResearchManager.FinishProject(def);
			}
		}
		



	}
}
