using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using BrendonBanville.Tools;

namespace BrendonBanville.Controllers
{
    [CustomEditor(typeof(TopDownCamera3D))]
    public class TopDownCamera3DEditor : Editor
    {
        private TopDownCamera3D camera { get { return target as TopDownCamera3D; } }
        private TopDownUserControl3D userControl { get { return target as TopDownUserControl3D; } }

        private TabsBlock tabs;

        private void OnEnable()
        {
            tabs = new TabsBlock(new Dictionary<string, System.Action>() 
            {
                {"Movement", MovementTab},
                {"Rotation", RotationTab},
                {"Height", HeightTab}
            });
            tabs.SetCurrentMethod(camera.lastTab);
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            Undo.RecordObject(camera, "RTS_CAmera");
            tabs.Draw();
            if (GUI.changed)
                camera.lastTab = tabs.curMethodIndex;
            EditorUtility.SetDirty(camera);
        }

        private void MovementTab()
        {
            using (new HorizontalBlock())
            {
                GUILayout.Label("Use keyboard input: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                userControl.useKeyboardInput = EditorGUILayout.Toggle(userControl.useKeyboardInput);
            }
            if(userControl.useKeyboardInput)
            {
                userControl.horizontalAxis = EditorGUILayout.TextField("Horizontal axis name: ", userControl.horizontalAxis);
                userControl.verticalAxis = EditorGUILayout.TextField("Vertical axis name: ", userControl.verticalAxis);
                camera.keyboardMovementSpeed = EditorGUILayout.FloatField("Movement speed: ", camera.keyboardMovementSpeed);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Screen edge input: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                userControl.useScreenEdgeInput = EditorGUILayout.Toggle(userControl.useScreenEdgeInput);
            }

            if(userControl.useScreenEdgeInput)
            {
                EditorGUILayout.FloatField("Screen edge border size: ", userControl.screenEdgeBorder);
                camera.screenEdgeMovementSpeed = EditorGUILayout.FloatField("Screen edge movement speed: ", camera.screenEdgeMovementSpeed);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Panning with mouse: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                userControl.usePanning = EditorGUILayout.Toggle(userControl.usePanning);
            }
            if(userControl.usePanning)
            {
                userControl.panningKey = (KeyCode)EditorGUILayout.EnumPopup("Panning when holding: ", userControl.panningKey);
                camera.panningSpeed = EditorGUILayout.FloatField("Panning speed: ", camera.panningSpeed);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Limit movement: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.limitMap = EditorGUILayout.Toggle(camera.limitMap);
            }
            if (camera.limitMap)
            {
                camera.limitX = EditorGUILayout.FloatField("Limit X: ", camera.limitX);
                camera.limitY = EditorGUILayout.FloatField("Limit Y: ", camera.limitY);
            }

            GUILayout.Label("Follow target", EditorStyles.boldLabel);
            camera.targetFollow = EditorGUILayout.ObjectField("Target to follow: ", camera.targetFollow, typeof(Transform), true) as Transform;
            camera.targetOffset = EditorGUILayout.Vector3Field("Target offset: ", camera.targetOffset);
            camera.followingSpeed = EditorGUILayout.FloatField("Following speed: ", camera.followingSpeed);
        }

        private void RotationTab()
        {
            using (new HorizontalBlock())
            {
                GUILayout.Label("Keyboard input: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                userControl.useKeyboardRotation = EditorGUILayout.Toggle(userControl.useKeyboardRotation);
            }
            if(userControl.useKeyboardRotation)
            {
                userControl.rotateLeftKey = (KeyCode)EditorGUILayout.EnumPopup("Rotate left: ", userControl.rotateLeftKey);
                userControl.rotateRightKey = (KeyCode)EditorGUILayout.EnumPopup("Rotate right: ", userControl.rotateRightKey);
                camera.rotationSped = EditorGUILayout.FloatField("Keyboard rotation speed", camera.rotationSped);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Mouse input: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                userControl.useMouseRotation = EditorGUILayout.Toggle(userControl.useMouseRotation);
            }
            if(userControl.useMouseRotation)
            {
                userControl.mouseRotationKey = (KeyCode)EditorGUILayout.EnumPopup("Mouse rotation key: ", userControl.mouseRotationKey);
                camera.mouseRotationSpeed = EditorGUILayout.FloatField("Mouse rotation speed: ", camera.mouseRotationSpeed);
            }
        }

        private void HeightTab()
        {
            using (new HorizontalBlock())
            {
                GUILayout.Label("Auto height: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                camera.autoHeight = EditorGUILayout.Toggle(camera.autoHeight);
            }
            if (camera.autoHeight)
            {
                camera.heightDampening = EditorGUILayout.FloatField("Height dampening: ", camera.heightDampening);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("groundMask"));
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Keyboard zooming: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                userControl.useKeyboardZooming = EditorGUILayout.Toggle(userControl.useKeyboardZooming);
            }
            if(userControl.useKeyboardZooming)
            {
                userControl.zoomInKey = (KeyCode)EditorGUILayout.EnumPopup("Zoom In: ", userControl.zoomInKey);
                userControl.zoomOutKey = (KeyCode)EditorGUILayout.EnumPopup("Zoom Out: ", userControl.zoomOutKey);
                camera.keyboardZoomingSensitivity = EditorGUILayout.FloatField("Keyboard sensitivity: ", camera.keyboardZoomingSensitivity);
            }

            using (new HorizontalBlock())
            {
                GUILayout.Label("Scrollwheel zooming: ", EditorStyles.boldLabel, GUILayout.Width(170f));
                userControl.useScrollwheelZooming = EditorGUILayout.Toggle(userControl.useScrollwheelZooming);
            }
            if (userControl.useScrollwheelZooming)
                camera.scrollWheelZoomingSensitivity = EditorGUILayout.FloatField("Scrollwheel sensitivity: ", camera.scrollWheelZoomingSensitivity);

            if (userControl.useScrollwheelZooming || userControl.useKeyboardZooming)
            {
                using (new HorizontalBlock())
                {
                    camera.maxHeight = EditorGUILayout.FloatField("Max height: ", camera.maxHeight);
                    camera.minHeight = EditorGUILayout.FloatField("Min height: ", camera.minHeight);
                }
            }  
        }
    }
}