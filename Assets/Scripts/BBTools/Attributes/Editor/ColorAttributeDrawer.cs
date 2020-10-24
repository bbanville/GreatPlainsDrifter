using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BrendonBanville.Tools
{
    [CustomPropertyDrawer(typeof(ColorAttribute))]
    public class ColorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color color = (attribute as ColorAttribute).color;
            Color prev = UnityEngine.GUI.color;
            UnityEngine.GUI.color = color;
            EditorGUI.PropertyField(position, property, label, true);
            UnityEngine.GUI.color = prev;
        }
    }
}
