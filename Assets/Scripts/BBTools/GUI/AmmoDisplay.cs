using UnityEngine;
using System.Collections;
using BrendonBanville.Tools;
using UnityEngine.UI;

namespace BrendonBanville.Tools.GUI
{	
	/// <summary>
	/// A class that combines a progress bar and a text display
	/// and that can be used to display the current ammo level of a weapon
	/// </summary>
	public class AmmoDisplay : ProgressBar 
	{
		/// the Text object used to display the current ammo numbers
		public Text TextDisplay;

		/// <summary>
		/// Updates the text display with the parameter string
		/// </summary>
		/// <param name="newText">New text.</param>
		public virtual void UpdateTextDisplay(string newText)
		{
			if (TextDisplay != null)
			{
				TextDisplay.text = newText;
			}
		}

		/// <summary>
		/// Updates the ammo display's text and progress bar
		/// </summary>
		/// <param name="totalAmmo">Total ammo.</param>
		/// <param name="maxAmmo">Max ammo.</param>
		/// <param name="ammoInMagazine">Ammo in magazine.</param>
		/// <param name="displayTotal">If set to <c>true</c> display total.</param>
		public virtual void UpdateAmmoDisplays(int totalAmmo, int maxAmmo, bool displayTotal)
		{
            this.UpdateBar(totalAmmo, 0, maxAmmo);
            this.UpdateTextDisplay(totalAmmo + "/" + maxAmmo);
        }
	}
}
