using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BrendonBanville.Tools;
using BrendonBanville.Tools.GUI;

namespace BrendonBanville.Achievements
{
	/// <summary>
	/// This class is used to display an achievement. Add it to a prefab containing all the required elements listed below.
	/// </summary>
	public class AchievementDisplayItem : MonoBehaviour 
	{		
		public Image BackgroundLocked;
		public Image BackgroundUnlocked;
		public Image Icon;
		public Text Title;
		public Text Description;
        public ProgressBar ProgressBarDisplay;
	}
}