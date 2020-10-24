using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BrendonBanville.Tools.GUI
{
	public class FPSUnlock : MonoBehaviour 
	{
		public int TargetFPS;

		protected virtual void Start()
		{
			Application.targetFrameRate = TargetFPS;
		}		
	}
}
