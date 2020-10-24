using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BrendonBanville.Tools.GUI;
using BrendonBanville.Tools;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Managers;

namespace BrendonBanville.Managers
{
    /// <summary>
    /// Handles all GUI effects and changes
    /// </summary>
    public class GUIManager : Singleton<GUIManager>
    {
        /// the main canvas
        public Canvas MainCanvas;
        /// the game object that contains the heads up display (avatar, health, points...)
        public GameObject HUD;
        /// the jetpack bar
        public ProgressBar[] HealthBars;
        /// the jetpack bar
        public RadialProgressBar[] DashBars;
        /// the panels and bars used to display current weapon ammo
        public AmmoDisplay[] AmmoDisplays;

        /// the pause screen game object
        public GameObject PauseScreen;
        /// the options screen game object
		public GameObject OptionsScreen;
        /// the death screen
        public GameObject DeathScreen;

        /// the options screen game object
		public GameObject DebugScreen;
        /// The debug mode alert
        public bool debugModeOn;

        public GameObject LetterBoxEffect;

        /// the key counter
        public GameObject KeyCounter;
        public TextMeshProUGUI CopperKeyDisplay;
        public TextMeshProUGUI IronKeyDisplay;
        private int copperKeys;
        private int ironKeys;
        Animator KeyCounterAnimator;

        protected float _initialJoystickAlpha;
        protected float _initialButtonsAlpha;

        private SoundManager SM;
        private bool BGMToggle = false;

        /// <summary>
        /// Initialization
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Initialization
        /// </summary>
        protected virtual void Start()
        {
            SetPauseScreen(false);
            SetDeathScreen(false);
            SetOptionsScreen(false);
            SetDebugScreen(false);

            SM = GameObject.FindGameObjectWithTag("AM").GetComponent<SoundManager>();
        }

        void Update()
        {
            if (GameManager.Instance.Paused)
            {
                PauseScreen.GetComponent<Animator>().Play("OpenPauseMenu");
            }
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
        /// Sets the avatar active or inactive
        /// </summary>
        /// <param name="state">If set to <c>true</c> turns the HUD active, turns it off otherwise.</param>
        public virtual void SetAvatarActive(bool state)
        {
            if (HUD != null)
            {
                HUD.SetActive(state);
            }
        }

        /// <summary>
        /// Sets the pause screen on or off.
        /// </summary>
        /// <param name="state">If set to <c>true</c>, sets the pause.</param>
        public virtual void SetPauseScreen(bool state)
        {
            if (PauseScreen != null)
            {
                SetOptionsScreen(false);
                PauseScreen.SetActive(state);
            }
        }

        /// <summary>
        /// Sets the pause screen on or off.
        /// </summary>
        /// <param name="state">If set to <c>true</c>, opens the options.</param>
        public virtual void SetOptionsScreen(bool state)
        {
            if (OptionsScreen != null)
            {
                OptionsScreen.SetActive(state);
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
                EventSystem.current.sendNavigationEvents = state;
            }
        }

        /// <summary>
        /// Sets the pause screen on or off.
        /// </summary>
        /// <param name="state">If set to <c>true</c>, opens the debug menu.</param>
        public virtual void SetDebugScreen(bool state)
        {
            if (DebugScreen != null)
            {
                DebugScreen.SetActive(state);
            }
        }

        public virtual void ToggleDebugMode(bool state)
        {
            state = !debugModeOn;
            debugModeOn = state;
        }

        /// <summary>
        /// Sets the jetpackbar active or not.
        /// </summary>
        /// <param name="state">If set to <c>true</c>, sets the pause.</param>
        public virtual void SetDashBar(bool state, string playerID)
        {
            if (DashBars == null)
            {
                return;
            }

            foreach (RadialProgressBar jetpackBar in DashBars)
            {
                if (jetpackBar != null)
                {
                    if (jetpackBar.PlayerID == playerID)
                    {
                        jetpackBar.gameObject.SetActive(state);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the ammo displays active or not
        /// </summary>
        /// <param name="state">If set to <c>true</c> state.</param>
        /// <param name="playerID">Player I.</param>
        public virtual void SetAmmoDisplays(bool state, string playerID)
        {
            if (AmmoDisplays == null)
            {
                return;
            }

            foreach (AmmoDisplay ammoDisplay in AmmoDisplays)
            {
                if (ammoDisplay != null)
                {
                    if (ammoDisplay.PlayerID == playerID)
                    {
                        ammoDisplay.gameObject.SetActive(state);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the health bar.
        /// </summary>
        /// <param name="currentHealth">Current health.</param>
        /// <param name="minHealth">Minimum health.</param>
        /// <param name="maxHealth">Max health.</param>
        /// <param name="playerID">Player I.</param>
        public virtual void UpdateHealthBar(float currentHealth, float minHealth, float maxHealth, string playerID)
        {
            if (HealthBars == null) { return; }
            if (HealthBars.Length <= 0) { return; }

            foreach (ProgressBar healthBar in HealthBars)
            {
                if (healthBar == null) { continue; }
                if (healthBar.PlayerID == playerID)
                {
                    healthBar.UpdateBar(currentHealth, minHealth, maxHealth);
                }
            }
        }

        /// <summary>
        /// Updates the jetpack bar.
        /// </summary>
        /// <param name="currentFuel">Current fuel.</param>
        /// <param name="minFuel">Minimum fuel.</param>
        /// <param name="maxFuel">Max fuel.</param>
        /// <param name="playerID">Player I.</param>
        public virtual void UpdateDashBars(float currentFuel, float minFuel, float maxFuel, string playerID)
        {
            if (DashBars == null)
            {
                return;
            }

            foreach (RadialProgressBar dashbar in DashBars)
            {
                if (dashbar == null) { return; }
                if (dashbar.PlayerID == playerID)
                {
                    dashbar.UpdateBar(currentFuel, minFuel, maxFuel);
                }
            }
        }

        public void MuteSFX(bool state)
        {
            SM.Settings.SfxOn = !state;
            if (state)
            {
                SM.audioMixer.SetFloat("sfxVol", -80);
            }
            else
            {
                SM.audioMixer.SetFloat("sfxVol", -10);
            }
        }

        public void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}