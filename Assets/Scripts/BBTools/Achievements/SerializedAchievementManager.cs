using UnityEngine;
using System.Collections;
using System;

namespace BrendonBanville.Tools
{
	[Serializable]
	/// <summary>
	/// A serializable class used to store an achievement into a save file
	/// </summary>
	public class SerializedAchievement
	{
		public string AchievementID;
		public bool UnlockedStatus;
		public int ProgressCurrent;

		/// <summary>
		/// Initializes a new instance of the <see cref="BrendonBanville.Tools.SerializedAchievement"/> class.
		/// </summary>
		/// <param name="achievementID">Achievement I.</param>
		/// <param name="unlockedStatus">If set to <c>true</c> unlocked status.</param>
		/// <param name="progressCurrent">Progress current.</param>
		public SerializedAchievement(string achievementID, bool unlockedStatus, int progressCurrent)
		{
			AchievementID = achievementID;
			UnlockedStatus = unlockedStatus;
			ProgressCurrent = progressCurrent;
		}
	}

	[Serializable]
	/// <summary>
	/// Serializable MM achievement manager.
	/// </summary>
	public class SerializedAchievementManager 
	{
		public SerializedAchievement[] Achievements;
	}
}