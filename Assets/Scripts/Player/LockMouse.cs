/*******************************************************************************
File: LockMouse.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    The the mouse cursor, and sets its visibility to false, while also enabling
    quitting from the Unity Editor via the escape button.
*******************************************************************************/

using UnityEngine;

namespace Assets.Scripts.Player
{
    public class LockMouse : MonoBehaviour
    {
        void Update()
        {
            //#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //#endif

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();

                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
        }
    }
}