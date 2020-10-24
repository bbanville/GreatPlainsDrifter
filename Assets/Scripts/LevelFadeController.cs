using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.Scripts.Managers;
using BrendonBanville.Tools;
using UnityEngine.SceneManagement;

public class LevelFadeController : Singleton<LevelFadeController>
{
    SoundManager SM;

    Image FaderImage;
    Color fadeColor = Color.black;

    public string SceneToLoad;
    [HideInInspector] public bool SceneLoaded;

    [SerializeField] List<string> soundsToPlayOnFadeIn;
    [SerializeField] float fadeInDuration = 4.0f;

    [SerializeField] List<string> soundsToPlayOnFadeOut;
    [SerializeField] float fadeOutDuration = 4.0f;

    [SerializeField] List<string> soundsToPlayOnReload;
    [SerializeField] float reloadDuration = 4.0f;

    public bool ExitGame = false;

    // Start is called before the first frame update
    void Start()
    {
        FaderImage = GameObject.Find("FadeImage").GetComponent<Image>();
        SM = GameObject.FindGameObjectWithTag("AM").GetComponent<SoundManager>();

        FadeIntoLevel();
    }

    void Update()
    {
        if (ExitGame)
        {
            Application.Quit();
        }
    }

    public void FadeIntoLevel()
    {
        foreach (string soundToPlay in soundsToPlayOnFadeIn)
        {
            SM.PlaySoundFromDict(soundToPlay);
        }

        fadeColor.a = 0;
        FaderImage.DOColor(fadeColor, fadeInDuration);
        SceneLoaded = true;
    }

    public void FadeOutOfLevel()
    {
        foreach (string soundToPlay in soundsToPlayOnFadeOut)
        {
            SM.PlaySoundFromDict(soundToPlay);
        }

        fadeColor.a = 1;
        FaderImage.DOColor(fadeColor, fadeOutDuration);
        SceneLoaded = false;
        SceneManager.LoadScene(SceneToLoad);
    }
    public void FadeOutOfLevel(string SceneName)
    {
        foreach (string soundToPlay in soundsToPlayOnFadeOut)
        {
            SM.PlaySoundFromDict(soundToPlay);
        }

        fadeColor.a = 1;
        FaderImage.DOColor(fadeColor, fadeOutDuration);
        SceneLoaded = false;
        SceneManager.LoadScene(SceneName);
    }

    public void ReloadLevel()
    {
        foreach (string soundToPlay in soundsToPlayOnReload)
        {
            SM.PlaySoundFromDict(soundToPlay);
        }

        fadeColor.a = 1;
        FaderImage.DOColor(fadeColor, reloadDuration);
        SceneLoaded = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
