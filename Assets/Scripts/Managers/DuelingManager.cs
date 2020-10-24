using Assets.Scripts.Weapons;
using BrendonBanville.Tools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class DuelingManager : Singleton<DuelingManager>
    {
        [Header("Tutorial")]
        public bool IsTutorialScene = false;
        public bool TutorialBlockActivated = false;

        // Tutorial block for teaching the player how to focus on the player
        public GameObject TutorialBlock1;
        public bool TutorialBlock1Complete = false;
        public float TimeToTutorialBlock1 = 3.0f;

        // Tutorial block for teaching the player how to draw and attack
        public GameObject TutorialBlock2;
        public bool TutorialBlock2Complete = false;
        public float TimeToTutorialBlock2 = 15.0f;

        [Header("General")]
        // The main canvas
        public Canvas DuelingHUDCanvas;
        // Grayscale shader for gun draw
        public Image GrayscaleShader;
        // Vignette used for player focus
        public Image FocusVignette;
        // The game object containing the heads up display
        public GameObject HUD;
        // The game object containing the draw prompt
        public GameObject DrawPrompt;

        // The pause screen game object
        public GameObject VictoryScreen;
        // The death screen
        public GameObject DeathScreen;

        public enum EndStates { Unsettled, PlayerWin, PlayerDeath }
        public EndStates endState = EndStates.Unsettled;

        [Header("HUD Variables")]
        // The clockfill object of the HUD
        public Image duelTimer;
        // The points counter under the timer
        public TextMeshProUGUI pointsText;
        bool clockActive;

        //float totalMilliSeconds = 0;
        //float TOTAL_MILLISECONDS = 0;
        float fillamount;

        [Header("Draw Times")]
        //public int minutes;
        //public int sec;
        //public int milsec;
        public float timeTillDraw = 7.6f;
        public float timeDuringDraw = 6.5f;
        [HideInInspector] public float TIME_DURING_DRAW;
        public float timeTillFadeOut = 2.5f;
        public Feedback DrawTimeFeedback;

        [Header("Character References")]
        // Reference to player
        public GameObject _player;
        // Reference to player's gun
        public BaseGun _playerGun;

        // Reference to player
        public GameObject _opponent;
        // Reference to player's gun
        public BaseGun _opponentGun;

        [Header("Player Stats / GUI")]
        [ReadOnly] [Range(0.0f, 1.0f)] public float playerFocus = 0;
        public TextMeshProUGUI focusCounter;
        [ReadOnly] [Range(0.0f, 1.0f)] public float timeSpeed = 1;
        public TextMeshProUGUI speedCounter;
        [HideInInspector] public int Points = 0;
        [HideInInspector] public int pointsForThisDuel;
        [HideInInspector] public int POINTS_FOR_THIS_DUEL;
        [HideInInspector] public bool gunDrawn;

        [Header("Opponent Stats")]
        [Range(0, 100)] public int opponentDifficulty;
        float baseBonus = 5;

        /// <summary>
        /// Variables used in producing UI feedback for user action
        /// </summary>
        [Header("Feedback")]
        public List<string> DeathAnims;

        public Feedback EnemyDeath;
        public Feedback PlayerDeath;

        /// <summary>
        /// Private references to external methods
        /// </summary>
        [Header("External References")]
        private SoundManager SM;

        private bool timerOn = false;
        private float Timer;

        /// <summary>
        /// Initialization
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
	    protected virtual void Start()
        {
            SetVictoryScreen(false);
            SetDeathScreen(false);

            SM = GameObject.FindGameObjectWithTag("AM").GetComponent<SoundManager>();

            TIME_DURING_DRAW = timeDuringDraw;
            UpdateHUDTimer();
            UpdateFocus();
            UpdateTime();

            SM.InDuel.TransitionTo(0.0f);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            UpdateFocus();
            UpdateTime();

            if (clockActive)
            {
                GrayscaleShader.gameObject.SetActive(true);

                if (SM.audioMixer.outputAudioMixerGroup == SM.InDuel_LowPass)
                {
                    SM.InDuel_LowPass.TransitionTo(0.5f);
                }
            }
            else
            {
                CalculateFocus();

                GrayscaleShader.gameObject.SetActive(false);

                if (SM.audioMixer.outputAudioMixerGroup == SM.InDuel)
                {
                    SM.InDuel.TransitionTo(0.5f);
                }
            }

            if (timerOn) { Timer += Time.deltaTime; }

            if (LevelFadeController.Instance.SceneLoaded && !TutorialBlockActivated) { timerOn = true; } else { timerOn = false; };

            #region Tutorial
            if (IsTutorialScene)
            {
                if (Timer >= TimeToTutorialBlock1 && !TutorialBlock1Complete)
                {
                    TutorialBlock1.SetActive(true);
                    TutorialBlockActivated = true;
                    TutorialBlock1Complete = true;
                }

                if (Timer >= TimeToTutorialBlock1 + TimeToTutorialBlock2 && !TutorialBlock2Complete)
                {
                    TutorialBlock2.SetActive(true);
                    TutorialBlockActivated = true;
                    TutorialBlock2Complete = true;
                }

                if (TutorialBlockActivated)
                {
                    if (Time.timeScale != 0.0f && !timerOn)
                    {
                        Time.timeScale = 0.0f;
                        timerOn = false;
                    }

                    if (Input.anyKeyDown)
                    {
                        TutorialBlock1.SetActive(false);
                        TutorialBlock2.SetActive(false);
                        TutorialBlockActivated = false;

                        Time.timeScale = 1.0f;
                        timerOn = true;
                    }
                }
            }
            #endregion

            #region Duel
            if (!TutorialBlockActivated)
            {
                if (TIME_DURING_DRAW <= 0)
                {
                    TIME_DURING_DRAW = 0;
                    clockActive = false;
                }

                if (clockActive)
                {
                    TIME_DURING_DRAW -= Time.deltaTime;
                    UpdateHUDTimer();
                }

                if (!gunDrawn)
                {
                    if (Timer >= timeTillDraw)
                    {
                        DrawPrompt.GetComponentInParent<Animator>().Play("PromptDraw");
                        DrawTimeFeedback.Play(_player.transform.position, this);
                        _opponent.GetComponent<Animator>().Play("DrawGunOpponent");

                        Time.timeScale = timeSpeed;
                    }

                    if (Timer >= timeTillDraw + 3.0f)
                    {
                        Time.timeScale = 1.0f;

                        if (!_opponent.GetComponentNoAlloc<Animator>().GetBool("Dying") && !_player.GetComponentNoAlloc<Animator>().GetBool("Dying"))
                        {
                            if (endState != EndStates.PlayerDeath)
                            {
                                _opponentGun.Fire();
                                KillPlayer();
                            }
                        }

                        if (Timer >= timeTillDraw + 3.0f + timeTillFadeOut)
                        {
                            Time.timeScale = 1.0f;

                            EndDuel();
                            Timer = 0;
                        }
                    }
                }
                else
                {
                    Color focusAlpha = new Color(0, 0, 0, 0);
                    FocusVignette.DOColor(focusAlpha, 0.01f);

                    if (SM.audioMixer.outputAudioMixerGroup != SM.InDuel_LowPass)
                    {
                        SM.InDuel_LowPass.TransitionTo(0.4f);
                    }

                    if (Timer >= timeTillDraw + timeDuringDraw)
                    {
                        Time.timeScale = 1.0f;

                        if (!_opponent.GetComponentNoAlloc<Animator>().GetBool("Dying") && !_player.GetComponentNoAlloc<Animator>().GetBool("Dying"))
                        {
                            if (endState != EndStates.PlayerDeath)
                            {
                                _opponentGun.Fire();
                                KillPlayer();
                            }
                        }

                        if (Timer >= timeTillDraw + timeDuringDraw + timeTillFadeOut)
                        {
                            Time.timeScale = 1.0f;

                            EndDuel();
                            Timer = 0;
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Sets the HUD active or inactive
        /// </summary>
        /// <param name="state">If set to <c>true</c> turns the HUD active, turns it off otherwise.</param>
        public virtual void SetHUDActive(bool state)
        {
            if (HUD != null)
            {
                HUD.SetActive(state);
            }
        }

        /// <summary>
        /// Sets the Draw Prompt active or inactive
        /// </summary>
        /// <param name="state">If set to <c>true</c> turns the Draw Prompt active, turns it off otherwise.</param>
        public virtual void SetDrawPromptActive(bool state)
        {
            if (DrawPrompt != null)
            {
                DrawPrompt.SetActive(state);
            }
        }

        /// <summary>
        /// Sets the death screen on or off.
        /// </summary>
        /// <param name="state">If set to <c>true</c>, sets the pause.</param>
        public virtual void SetVictoryScreen(bool state)
        {
            if (VictoryScreen != null)
            {
                VictoryScreen.SetActive(state);
            }
        }

        /// <summary>
        /// Sets the death screen on or off.
        /// </summary>
        /// <param name="state">If set to <c>true</c>, sets the pause.</param>
        public virtual void SetDeathScreen(bool state)
        {
            if (DeathScreen != null)
            {
                DeathScreen.SetActive(state);
            }
        }

        public void UpdateFocus()
        {
            focusCounter.text = Mathf.RoundToInt(playerFocus * 100) + "%";
        }

        public void IncreasePlayerFocus(float valueToAdd)
        {
            playerFocus += valueToAdd;
            playerFocus = Mathf.Clamp(playerFocus, 0.0f, 1.0f);
        }

        public void DecreasePlayerFocus(float valueToSubtract)
        {
            playerFocus -= valueToSubtract;
            playerFocus = Mathf.Clamp(playerFocus, 0.0f, 1.0f);
        }

        public void UpdateTime()
        {
            float timeScaleFromFocus = Mathf.Abs(playerFocus - 1.0f);
            timeSpeed = Mathf.Clamp(timeScaleFromFocus, 0.2f, 1.0f);

            speedCounter.text = Mathf.RoundToInt(timeSpeed * 100) + "%";
        }

        public void UpdateHUDTimer()
        {
            pointsText.text = PointsFromTime().ToString();
            fillLoading();
        }

        public int PointsFromTime()
        {
            int result;

            result = Mathf.RoundToInt(TIME_DURING_DRAW * 1000 / 4);

            return result;
        }

        public virtual void OnNUEvent(NUTimeScaleEvent timeScaleEvent)
        {

        }

        public void DrawGun()
        {
            var _playerAnimator = _player.GetComponent<Animator>();
            _playerAnimator.Play("DrawGun");

            var _cameraAnimator = Camera.main.GetComponent<Animator>();
            _cameraAnimator.Play("PlayerDraw");

            SM.PlaySoundFromDict("DrawFromLeather_01");
            SM.PlaySoundFromDict("ExplosiveDraw");

            clockActive = true;
            gunDrawn = true;
        }

        public void KillOpponent()
        {
            SM.InDuel.TransitionTo(0.1f);

            EnemyDeath.FeedbackAnimator = _opponent.GetComponent<Animator>();
            EnemyDeath.FeedbackAnimator.SetInteger("DeathState", 1);
            EnemyDeath.FeedbackAnimator.SetBool("Dying", true);
            EnemyDeath.AnimatorTriggerParameterName = null;
            endState = EndStates.PlayerWin;

            EnemyDeath.Play(_opponent.transform.position, this);

            Points = PointsFromTime();
            clockActive = false;

            Time.timeScale = 1.0f;
            SM.PlaySoundFromDict("EndShot");
        }

        public void KillPlayer()
        {
            SM.InDuel.TransitionTo(0.1f);

            PlayerDeath.FeedbackAnimator = _player.GetComponent<Animator>();
            PlayerDeath.FeedbackAnimator.Play("PlayerDeath");

            PlayerDeath.FeedbackAnimator = GetComponent<Animator>();
            PlayerDeath.FeedbackAnimator.Play("PlayerHitVignette");

            endState = EndStates.PlayerDeath;
            PlayerDeath.Play(_player.transform.position, this);

            Time.timeScale = 1.0f;
            SM.PlaySoundFromDict("EndShot");
        }

        public void EndDuel()
        {
            if (endState == EndStates.PlayerWin)
            {
                LevelFadeController.Instance.FadeOutOfLevel();
            }

            if (endState == EndStates.PlayerDeath)
            {
                LevelFadeController.Instance.ReloadLevel();
            }
        }

        void fillLoading()
        {
            float fill = (float)pointsForThisDuel / POINTS_FOR_THIS_DUEL;
            duelTimer.fillAmount = fill;
        }

        void CalculateFocus()
        {
            if (playerFocus > 0)
            {
                Color focusAlpha = new Color(0, 0, 0, playerFocus);
                FocusVignette.DOColor(focusAlpha, 0.01f);
            }
        }
    }
}
