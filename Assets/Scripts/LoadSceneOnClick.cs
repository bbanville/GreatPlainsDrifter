/*******************************************************************************
File:      LoadSceneOnClick.cs
Author:    Brendon Banville
DP Email:  b.banville@digipen.edu
Date:      4/8/2019
Course:    CS175
Section:   A

Description:
    Contains the function for loading a new scene.  
*******************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public void LoadByIndex(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}