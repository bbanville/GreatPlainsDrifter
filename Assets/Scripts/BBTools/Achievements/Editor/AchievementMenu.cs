using UnityEngine;
using System.Collections;
using BrendonBanville.Tools;
using UnityEditor;

namespace BrendonBanville.Tools
{	
	public static class MMAchievementMenu 
	{
		[MenuItem("Tools/BBTools/Reset all achievements", false,21)]
		/// <summary>
		/// Adds a menu item to enable help
		/// </summary>
		private static void EnableHelpInInspectors()
		{
			AchievementManager.ResetAllAchievements ();
		}
	}
}