/*******************************************************************************
File: LockMouse.cs
Author: Brendon Banville
Email: brendonbanville@gmail.com
Last Updated: 9/17/2019
Description:
    The the mouse cursor, and sets its visibility to false, while also enabling
    quitting from the Unity Editor via the escape button.
*******************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

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
                SceneManager.LoadScene(0);

                //Application.Quit();
                //
                //#if UNITY_EDITOR
                //UnityEditor.EditorApplication.isPlaying = false;
                //#endif
            }
        }
    }
}