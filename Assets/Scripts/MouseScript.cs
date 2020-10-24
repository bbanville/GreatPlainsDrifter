/*******************************************************************************
File:      MouseScript.cs
Author:    Brendon Banville
DP Email:  b.banville@digipen.edu
Date:      4/8/2019
Course:    CS175
Section:   A

Description:
    Contains the functions and methods by which the mouse is tracked in game
    and which cursor to use during certain scenes.  
*******************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MouseScript : MonoBehaviour
{

    public List<Texture2D> cursorTextures = new List<Texture2D>();
    private Vector2 cursorHotspot;

    /// initialize mouse with a new texture with the
    /// hotspot set to the middle of the texture
    /// (don't forget to set the texture in the inspector
    /// in the editor)
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "AncestryMainMenu")
        {
            Cursor.SetCursor(cursorTextures[1], cursorHotspot, CursorMode.Auto);
        }
        else if (SceneManager.GetActiveScene().name == "scn_testing_grounds")
        {
            cursorHotspot = new Vector2(cursorTextures[0].width / 2, cursorTextures[0].height / 2);
            Cursor.SetCursor(cursorTextures[0], cursorHotspot, CursorMode.Auto);
        }
    }

    /// To check where your mouse is really pointing
    /// track the mouse position in you update function
    void Update()
    {
        Vector3 currentMouse = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(currentMouse);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        Debug.DrawLine(ray.origin, hit.point);
    }
}