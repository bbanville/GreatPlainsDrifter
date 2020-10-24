using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEditor;

namespace BrendonBanville.Tools
{	

	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]

	public class ReadOnlyAttributeDrawer : PropertyDrawer
	{
	    // Necessary since some properties tend to collapse smaller than their content
	    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	    {
	        return EditorGUI.GetPropertyHeight(property, label, true);
	    }

	    // Draw a disabled property field
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	    {
            UnityEngine.GUI.enabled = false; // Disable fields
	        EditorGUI.PropertyField(position, property, label, true);
            UnityEngine.GUI.enabled = true; // Enable fields
	    }
	}
}