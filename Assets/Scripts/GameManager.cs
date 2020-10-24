using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BrendonBanville.Managers;
using Assets.Scripts.Managers;

namespace BrendonBanville.Tools
{
    /// <summary>
    /// A list of the possible TopDown Engine base events
    /// </summary>
    public enum EngineEventTypes
    {
        LevelStart,
        LevelComplete,
        LevelEnd,
        Pause,
        UnPause,
        PlayerDeath,
        RespawnStarted,
        RespawnComplete,
        GameOver
    }

    /// <summary>
    /// A type of events used to signal level start and end (for now)
    /// </summary>
    public struct EngineEvent
    {
        public EngineEventTypes EventType;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrendonBanville.Tools.EngineEvent"/> struct.
        /// </summary>
        /// <param name="eventType">Event type.</param>
        public EngineEvent(EngineEventTypes eventType)
        {
            EventType = eventType;
        }

        static EngineEvent e;
        public static void Trigger(EngineEventTypes eventType)
        {
            e.EventType = eventType;
            NUEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// A list of the methods available to change the current score
    /// </summary>
    public enum PointsMethods
    {
        Add,
        Set
    }

    /// <summary>
    /// A type of event used to signal changes to the current score
    /// </summary>
    public struct TopDownEnginePointEvent
    {
        public PointsMethods PointsMethod;
        public int Points;
        /// <summary>
        /// Initializes a new instance of the <see cref="MoreMountains.TopDownEngine.TopDownEnginePointEvent"/> struct.
        /// </summary>
        /// <param name="pointsMethod">Points method.</param>
        /// <param name="points">Points.</param>
        public TopDownEnginePointEvent(PointsMethods pointsMethod, int points)
        {
            PointsMethod = pointsMethod;
            Points = points;
        }

        static TopDownEnginePointEvent e;
        public static void Trigger(PointsMethods pointsMethod, int points)
        {
            e.PointsMethod = pointsMethod;
            e.Points = points;
            NUEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// A list of the possible pause methods
    /// </summary>
	public enum PauseMethods
    {
        PauseMenu,
        NoPauseMenu
    }

    /// <summary>
    /// The game manager is a persistent singleton that handles points and time
    /// </summary>
    [AddComponentMenu("TopDown Engine/Managers/Game Manager")]
    public class GameManager : PersistentSingleton<GameManager>, NUEventListener<EngineEvent>
    {
        /// the target frame rate for the game
        public int TargetFrameRate = 300;
        
        /// the current number of game points
        public int Points { get; private set; }
        /// true if the game is currently paused
        public bool Paused { get; set; }

        public GameObject faderObj;
        public Image faderImg;
        public float fadeSpeed = .02f;
        private Color fadeTransparency = new Color(0, 0, 0, .04f);
        public bool fadedIn = false;

        [Header("Load Scene Async")]
        public string sceneToLoad;
        [ReadOnly] public bool asyncLoaded = false;
        public AsyncOperation asyncOperation;

        private SoundManager SM;

        [Header("User Inputs")]
        public KeyCode pauseButton = KeyCode.Escape;

        // storage
        protected Stack<float> _savedTimeScale;
        protected bool _inventoryOpen = false;
        protected bool _pauseMenuOpen = false;
        protected int _initialMaximumLives;
        protected int _initialCurrentLives;

        /// <summary>
        /// On Awake we initialize our list of points of entry
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// On Start(), sets the target framerate to whatever's been specified
        /// </summary>
        protected virtual void Start()
        {
            //Cursor.SetCursor()
            Application.targetFrameRate = TargetFrameRate;
            _savedTimeScale = new Stack<float>();

            SM = GameObject.FindGameObjectWithTag("AM").GetComponent<SoundManager>();

            if (!SceneManager.GetActiveScene().name.Contains("Duel") && !SceneManager.GetActiveScene().name.Contains("Menu"))
            {
                SM.Outside.TransitionTo(1.0f);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(pauseButton))
            {
                if (Paused && GUIManager.Instance.OptionsScreen.activeInHierarchy)
                {
                    // Close Pause Menu
                }
                else if (Paused)
                {
                    EngineEvent.Trigger(EngineEventTypes.UnPause);
                }
                else if (!Paused)
                {
                    EngineEvent.Trigger(EngineEventTypes.Pause);
                }
            }
        }

        /// <summary>
        /// this method resets the whole game manager
        /// </summary>
        public virtual void Reset()
        {
            Points = 0;
            Time.timeScale = 1f;
            Paused = false;
        }

        /// <summary>
        /// Adds the points in parameters to the current game points.
        /// </summary>
        /// <param name="pointsToAdd">Points to add.</param>
        public virtual void AddPoints(int pointsToAdd)
        {
            Points += pointsToAdd;
        }

        /// <summary>
        /// use this to set the current points to the one you pass as a parameter
        /// </summary>
        /// <param name="points">Points.</param>
        public virtual void SetPoints(int points)
        {
            Points = points;
        }

        /// <summary>
        /// sets the timescale to the one in parameters
        /// </summary>
        /// <param name="newTimeScale">New time scale.</param>
        public virtual void SetTimeScale(float newTimeScale)
        {
            _savedTimeScale.Push(Time.timeScale);
            Time.timeScale = newTimeScale;
        }

        /// <summary>
        /// Resets the time scale to the last saved time scale.
        /// </summary>
        public virtual void ResetTimeScale()
        {
            if (_savedTimeScale.Count > 0)
            {
                Time.timeScale = _savedTimeScale.Peek();
                _savedTimeScale.Pop();
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        /// <summary>
        /// Pauses the game or unpauses it depending on the current state
        /// </summary>
        public virtual void Pause(PauseMethods pauseMethod = PauseMethods.PauseMenu)
        {
            if ((pauseMethod == PauseMethods.PauseMenu) && _inventoryOpen)
            {
                return;
            }

            // if time is not already stopped		
            if (Time.timeScale > 0.0f)
            {
                NUTimeScaleEvent.Trigger(NUTimeScaleMethods.For, 0f, 0f, false, 0f, true);
                Instance.Paused = true;
                if ((GUIManager.Instance != null) && (pauseMethod == PauseMethods.PauseMenu))
                {
                    GUIManager.Instance.SetPauseScreen(true);
                    _pauseMenuOpen = true;
                }
                if (pauseMethod == PauseMethods.NoPauseMenu)
                {
                    _inventoryOpen = true;
                }
            }
            else
            {
                UnPause(pauseMethod);
            }
        }

        /// <summary>
        /// Unpauses the game
        /// </summary>
        public virtual void UnPause(PauseMethods pauseMethod = PauseMethods.PauseMenu)
        {
            NUTimeScaleEvent.Trigger(NUTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
            Instance.Paused = false;
            if ((GUIManager.Instance != null) && (pauseMethod == PauseMethods.PauseMenu))
            {
                GUIManager.Instance.SetPauseScreen(false);
                _pauseMenuOpen = false;
            }
            if (_inventoryOpen)
            {
                _inventoryOpen = false;
            }
        }

        /// <summary>
        /// Catches TopDownEngineEvents and acts on them, playing the corresponding sounds
        /// </summary>
        /// <param name="engineEvent">TopDownEngineEvent event.</param>
        public virtual void OnNUEvent(EngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case EngineEventTypes.Pause:
                    Pause();
                    break;

                case EngineEventTypes.UnPause:
                    UnPause();
                    break;
            }
        }

        ///// <summary>
        ///// OnDisable, we start listening to events.
        ///// </summary>
        //protected virtual void OnEnable()
        //{
        //    this.NUEventStartListening<NUGameEvent>();
        //}
        //
        ///// <summary>
        ///// OnDisable, we stop listening to events.
        ///// </summary>
        //protected virtual void OnDisable()
        //{
        //    this.NUEventStopListening<NUGameEvent>();
        //}

        IEnumerator FadeOutToAsyncScene(GameObject faderObject, Image fader, AsyncOperation asyncOperation)
        {
            faderObject.SetActive(true);
            while (fader.color.a < 1)
            {
                fader.color += fadeTransparency;
                yield return new WaitForSeconds(fadeSpeed);
            }
            fadedIn = false;
            asyncOperation.allowSceneActivation = true;
        }

        IEnumerator LoadScene()
        {
            yield return null;

            //Begin to load the Scene you specify
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;
            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
            {
                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f)
                {
                    asyncLoaded = true;
                }

                yield return null;
            }
        }

        public void LoadLastCheckpoint()
        {

        }

        public void FadeOutToAsyncScene(AsyncOperation asyncOperation)
        {
            StartCoroutine(FadeOutToAsyncScene(Instance.faderObj, Instance.faderImg, asyncOperation));
        }
    }
}