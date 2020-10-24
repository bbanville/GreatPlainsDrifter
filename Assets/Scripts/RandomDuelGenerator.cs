using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomDuelGenerator : MonoBehaviour
{
    Scene BaseScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveRandomDuel(Scene newDuelScene)
    {
        
    }

    public void LoadRandomDuel(string newDuelSceneName)
    {
        SceneManager.LoadScene(newDuelSceneName);
    }
}
