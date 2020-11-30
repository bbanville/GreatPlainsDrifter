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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string SceneToLoad = "";

    [Header("Activation Method")]
    public bool ActivateOnTriggerEnter = false;

    private void OnTriggerEnter(Collider other)
    {
        if (ActivateOnTriggerEnter)
        {
            LoadByName(SceneToLoad);
        }
    }

    public void LoadByIndex(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    public void LoadByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
