using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BrendonBanville.Tools;

namespace BrendonBanville.Achievements
{
	/// <summary>
	/// An event type used to broadcast the fact that an achievement has been unlocked
	/// </summary>
	public struct AchievementUnlockedEvent
	{
		/// the achievement that has been unlocked
		public Achievement Achievement;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="newAchievement">New achievement.</param>
		public AchievementUnlockedEvent(Achievement newAchievement)
		{
			Achievement = newAchievement;
        }

        static AchievementUnlockedEvent e;
        public static void Trigger(Achievement newAchievement)
        {
            e.Achievement = newAchievement;
            NUEventManager.TriggerEvent(e);
        }
    }
}