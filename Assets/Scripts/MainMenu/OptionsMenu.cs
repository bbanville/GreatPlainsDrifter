/*******************************************************************************
File:      OptionsMenu.cs
Author:    Brendon Banville
DP Email:  b.banville@digipen.edu
Date:      4/3/2019
Course:    CS175
Section:   A

Description:
    This file contains the functions and behaviour for updating the options menu
    and the system settings it controls.  
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BrendonBanville.Managers;
using Assets.Scripts.Managers;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Options")]
    ///audio options references
    public Slider musicSlider;
    public TextMeshProUGUI musicVolText;
    public Slider sfxSlider;
    public TextMeshProUGUI sfxVolText;
    public Slider uiSlider;
    public TextMeshProUGUI uiVolText;

    public SoundManager SM;

    [Header("Graphic Options")]
    ///graphic options references
    public TextMeshProUGUI resText;
    public Toggle fullScreenToggle;
    public Toggle vsyncToggle;
    public Toggle muteSFXToggle;
    public Toggle muteBGMToggle;

    public List<ResolutionItem> resolutions = new List<ResolutionItem>();
    public int selectedRes;

    [Header("Gameplay Options")]
    ///gameplay options references
    public Slider hatSizeSlider;
    public TextMeshProUGUI hatSizeText;
    public TextMeshProUGUI drunkText;
    public List<DrunkLevel> drunkLevels = new List<DrunkLevel>();
    public int selectedDrunkLevel;
    drunk drunkScript;

    /// Use this for initialization
    void Start()
    {
        if (SM == null)
        {
            SM = FindObjectOfType<SoundManager>();
        }

        drunkScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<drunk>();

        ///Set starting audio values
        //musicSlider.value = AM.GetMusicVol() * 100f;
        //sfxSlider.value = AM.GetSFXVol() * 100f;
        //uiSlider.value = AM.GetSFXVol() * 100f;
        UpdateVolLabels();

        ///set starting graphics values
        fullScreenToggle.isOn = Screen.fullScreen;
        vsyncToggle.isOn = QualitySettings.vSyncCount == 0 ? false : true;

        bool foundRes = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].width && Screen.height == resolutions[i].height)
            {
                selectedRes = i;
                foundRes = true;
            }
        }
        if (!foundRes)
        {
            resolutions.Add(new ResolutionItem(Screen.width, Screen.height));
            selectedRes = resolutions.Count - 1;
        }

        resText.text = resolutions[selectedRes].width + " x " + resolutions[selectedRes].height;
    }

    ///Update is called once per frame
    void Update()
    {

    }

    public void UpdateVolLabels()
    {
        musicVolText.text = Mathf.Round(musicSlider.value).ToString();
        sfxVolText.text = Mathf.Round(sfxSlider.value).ToString();
        uiVolText.text = Mathf.Round(uiSlider.value).ToString();
        hatSizeText.text = Mathf.Round(hatSizeSlider.value).ToString();
    }

    public void AdjustMusicVol()
    {
        SM.SetMusicVol(musicSlider.value - 80.0f);
        //SM.SetMusicVol(ConvertToDecibel(musicSlider.value));
        UpdateVolLabels();
    }

    public void AdjustSFXVol()
    {
        SM.SetSFXVol(sfxSlider.value / 100.0f);
        //SM.SetSFXVol(ConvertToDecibel(sfxSlider.value));
        UpdateVolLabels();
    }

    public void AdjustHatSize()
    {
        UpdateVolLabels();
    }

    public void ResSelectLeft()
    {
        if (selectedRes > 0)
        {
            selectedRes--;
        }

        resText.text = resolutions[selectedRes].width + " x " + resolutions[selectedRes].height;
    }

    public void ResSelectRight()
    {
        if (selectedRes < resolutions.Count - 1)
        {
            selectedRes++;
        }

        resText.text = resolutions[selectedRes].width + " x " + resolutions[selectedRes].height;
    }

    public void ApplyResolution()
    {
        Screen.SetResolution(resolutions[selectedRes].width, resolutions[selectedRes].height, fullScreenToggle.isOn);
    }

    public void DrunkSelectLeft()
    {
        if (selectedDrunkLevel > 0)
        {
            selectedDrunkLevel--;
        }

        drunkText.text = drunkLevels[selectedDrunkLevel].LevelName;
    }

    public void DrunkSelectRight()
    {
        if (selectedDrunkLevel < drunkLevels.Count - 1)
        {
            selectedDrunkLevel++;
        }

        drunkText.text = drunkLevels[selectedDrunkLevel].LevelName;
    }

    public void ApplyDrunkLevel()
    {
        if (drunkLevels[selectedDrunkLevel].LevelName == "Sober")
        {
            drunkScript.enabled = false;
        }
        else
        {
            if (drunkScript.enabled == false)
            {
                drunkScript.enabled = true;
            }

            if (drunkLevels[selectedDrunkLevel].DrunkMaterial != null)
            {
                drunkScript.activeMaterial = drunkLevels[selectedDrunkLevel].DrunkMaterial;
            }
        }
    }

    public void SetFullScreen()
    {
        if (fullScreenToggle.isOn)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }

    public void SetVsync()
    {
        if (vsyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void SetMuteMusic()
    {
        if (muteBGMToggle.isOn)
        {
            SM.Settings.SfxOn = false;
            SM.audioMixer.SetFloat("sfxVol", -80);
        }
        else
        {
            SM.Settings.SfxOn = true;
            SM.audioMixer.SetFloat("sfxVol", -10);
        }
    }

    public void SetMuteSfx()
    {
        if (muteSFXToggle.isOn)
        {
            SM.musicOff.TransitionTo(0.1f);
        }
        else
        {
            SM.musicOn.TransitionTo(0.1f);
        }
    }

    public void SetHatSize()
    {
        
    }

    public void ApplyChanges()
    {
        ApplyResolution();
        ApplyDrunkLevel();
        SetFullScreen();
        SetVsync();
        //SetMuteMusic();
        //SetMuteSfx();
        GUIManager.Instance.SetOptionsScreen(false);

        ///make sure audio changes will be applied on exit
        //AM.SaveData();
    }

    public void CloseMenu()
    {
        ///reset audioLevels based saved data. This will ensure if changes aren't applied in menu, audio will revert to last setting
        //AM.LoadData();
        gameObject.SetActive(false);
    }

    public float ConvertToDecibel(float _value){
         return Mathf.Log10(Mathf.Max(_value, 0.0001f))*20f;
     }
}