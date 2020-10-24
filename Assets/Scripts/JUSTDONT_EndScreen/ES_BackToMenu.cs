using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ES_BackToMenu : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("MainMenu_Saloon");
    }
}
