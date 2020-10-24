/*******************************************************************************
File:      QuitOnClick.cs
Author:    Brendon Banville
DP Email:  b.banville@digipen.edu
Date:      4/8/2019
Course:    CS175
Section:   A

Description:
    Contains the function by which the game is quit and how it is quit based
    upon whether it is the unity player or editor.  
*******************************************************************************/

using UnityEngine;
using System.Collections;

public class QuitOnClick : MonoBehaviour
{

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }

}