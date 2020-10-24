/*******************************************************************************
File:      MainMenu.cs
Author:    Brendon Banville
DP Email:  b.banville@digipen.edu
Date:      4/3/2019
Course:    CS175
Section:   A

Description:
    This file contains the functions and behaviour for updating and controlling
    the main menu.  
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Managers;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [Header("General")]
        public string newGameScene;
        public string continueScene;

        private SoundManager SM;

        public Animator menuAnimator;
        public GameObject gameTitle;

        [Header("Main")]
        public GameObject mainScreen;
        public GameObject mainWorldSpace;

        [Header("Options")]
        public GameObject optionsScreen;
        public GameObject optionsForZoom;
        public GameObject optionsForAfterZoom;

        [Header("Credits")]
        public GameObject creditsScreen;
        public Image creditsWorldSpace;
        public Sprite creditsForZoom;
        public Sprite creditsForAfterZoom;

        [Header("Quit")]
        public GameObject quitConfirmMenu;
        public GameObject quitButton;

        [Header("Environment Objects")]
        public List<EnvironmentMenuObject> envObjects;

        // Use this for initialization
        void Start()
        {
            SM = GameObject.FindGameObjectWithTag("AM").GetComponent<SoundManager>();
        }

        //Update is called once per frame
        void Update()
        {

        }

        #region Play
        //Start New Game
        public void NewGame()
        {
            //Check if a scene name has been entered to load
            if (newGameScene.Equals("") || newGameScene.Equals(string.Empty))
            {
                Debug.LogError("No scene name entered to load for New Game");
            }
            else
            {
                menuAnimator.Play("ToSaloon");
                mainScreen.GetComponent<CanvasGroup>().alpha = 0;
                mainScreen.GetComponent<CanvasGroup>().interactable = false;
                mainWorldSpace.SetActive(true);
                StartCoroutine(ZoomIntoDoors(0.8f));
            }
        }

        //Continue Game (Ideally used for loading to a level Select)
        public void ContinueGame()
        {
            //Check if a scene name has been entered to load
            if (continueScene.Equals("") || continueScene.Equals(string.Empty))
            {
                Debug.LogError("No scene name entered to load for Continue");
            }
            else
            {

            }
        }

        IEnumerator ZoomIntoDoors(float time)
        {
            yield return new WaitForSeconds(0.9f);

            SM.PlaySoundFromDict("WhipCrack");

            yield return new WaitForSeconds(time);

            SceneManager.LoadScene(newGameScene);
        }
        #endregion

        #region Options
        //Open Options Menu
        public void OpenOptions()
        {
            menuAnimator.Play("ToOptionsMenu2");
            mainScreen.GetComponent<CanvasGroup>().alpha = 0;
            mainScreen.GetComponent<CanvasGroup>().interactable = false;
            mainWorldSpace.SetActive(true);
            StartCoroutine(OpenOptionsMenu(1.25f));
        }

        IEnumerator OpenOptionsMenu(float time)
        {
            yield return new WaitForSeconds(time);

            optionsForZoom.SetActive(false);
            optionsForAfterZoom.SetActive(true);
            optionsScreen.SetActive(true);
            mainScreen.SetActive(false);
            mainScreen.GetComponent<CanvasGroup>().alpha = 1;
        }

        //Close options Panel
        public void CloseOptions()
        {
            menuAnimator.Play("FromOptionsMenu2");
            optionsForZoom.SetActive(true);
            optionsForAfterZoom.SetActive(false);
            optionsScreen.GetComponent<CanvasGroup>().alpha = 0;
            StartCoroutine(CloseOptionsMenu(1.25f));
        }

        IEnumerator CloseOptionsMenu(float time)
        {
            yield return new WaitForSeconds(time);

            mainScreen.GetComponent<CanvasGroup>().interactable = true;
            mainScreen.SetActive(true);
            mainWorldSpace.SetActive(false);
            optionsScreen.SetActive(false);
            optionsScreen.GetComponent<CanvasGroup>().alpha = 1;
        }
        #endregion

        #region Credits
        //Open Credits Panel
        public void OpenCredits()
        {
            menuAnimator.Play("ToCredits");
            mainScreen.GetComponent<CanvasGroup>().alpha = 0;
            mainWorldSpace.SetActive(true);
            StartCoroutine(OpenCreditsMenu(1.25f));
        }

        IEnumerator OpenCreditsMenu(float time)
        {
            yield return new WaitForSeconds(time);

            creditsScreen.SetActive(true);
            creditsWorldSpace.sprite = creditsForAfterZoom;
            mainScreen.SetActive(false);
            mainScreen.GetComponent<CanvasGroup>().alpha = 1;
        }

        //Close Credits Panel
        public void CloseCredits()
        {
            menuAnimator.Play("FromCredits");
            creditsWorldSpace.sprite = creditsForZoom;
            creditsScreen.GetComponent<CanvasGroup>().alpha = 0;
            StartCoroutine(CloseCreditsMenu(1.25f));
        }

        IEnumerator CloseCreditsMenu(float time)
        {
            yield return new WaitForSeconds(time);

            mainScreen.SetActive(true);
            mainWorldSpace.SetActive(false);
            creditsScreen.SetActive(false);
            creditsScreen.GetComponent<CanvasGroup>().alpha = 1;
        }
        #endregion

        //Open Quit Confirm Dialog
        public void PressQuit()
        {
            quitButton.SetActive(false);
            SM.PlaySoundFromDict("Ricochet1");
            quitConfirmMenu.SetActive(true);
        }

        //Exit the game
        public void ConfirmQuit()
        {
            mainScreen.GetComponent<CanvasGroup>().alpha = 0;
            mainScreen.GetComponent<CanvasGroup>().interactable = false;
            //mainScreen.SetActive(false);
            SM.PlaySoundFromDict("WhipCrack");
            menuAnimator.Play("SlapOut");
            StartCoroutine(ConfirmQuitApplication(0.3f));
        }

        IEnumerator ConfirmQuitApplication(float time)
        {
            yield return new WaitForSeconds(time);

            Application.Quit();
        }

        //Cancel quitting the game
        public void CancelQuit()
        {
            SM.PlaySoundFromDict("CylinderSpin");
            quitButton.SetActive(true);
            quitConfirmMenu.SetActive(false);
        }

        public void PlayButtonSound()
        {

        }

        public void InteractEnvObject(string envObjectName)
        {
            foreach (EnvironmentMenuObject envObject in envObjects)
            {
                if (envObject.Name == envObjectName)
                {
                    envObject.ObjectAnimator.Play(envObjectName + "Interact");
                }
            }
        }
    }
}