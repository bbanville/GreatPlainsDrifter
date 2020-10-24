using UnityEngine;
using BrendonBanville.Tools;
using UnityEditor;
using UnityEngine.UI;
using BrendonBanville.Tools.GUI;

namespace BrendonBanville.Tools
{	
	[CustomEditor(typeof(GUI.HealthBar),true)]
	/// <summary>
	/// Custom editor for health bars (mostly a switch for prefab based / drawn bars)
	/// </summary>
	public class HealthBarEditor : Editor 
	{
		public GUI.HealthBar HealthBarTarget 
		{ 
			get 
			{ 
				return (GUI.HealthBar)target;
			}
		} 

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (HealthBarTarget.HealthBarType == GUI.HealthBar.HealthBarTypes.Prefab)
			{
				Editor.DrawPropertiesExcluding(serializedObject, new string[] {"Size","BackgroundPadding", "SortingLayerName", "ForegroundColor", "DelayedColor", "BorderColor", "BackgroundColor", "Delay", "LerpFrontBar", "LerpFrontBarSpeed", "LerpDelayedBar", "LerpDelayedBarSpeed", "BumpScaleOnChange", "BumpDuration", "BumpAnimationCurve" });
            }

			if (HealthBarTarget.HealthBarType == GUI.HealthBar.HealthBarTypes.Drawn)
			{
				Editor.DrawPropertiesExcluding(serializedObject, new string[] {"HealthBarPrefab" });
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
}