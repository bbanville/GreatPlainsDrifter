using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using BrendonBanville.Tools;

namespace BrendonBanville.Tools
{
    /// <summary>
    /// Events used to trigger faders on or off
    /// </summary>
    public struct FadeEvent
    {
        public float Duration;
        public float TargetAlpha;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrendonBanville.MMInterface.MMFadeEvent"/> struct.
        /// </summary>
        /// <param name="duration">Duration, in seconds.</param>
        /// <param name="targetAlpha">Target alpha, from 0 to 1.</param>
        public FadeEvent(float duration, float targetAlpha)
        {
            Duration = duration;
            TargetAlpha = targetAlpha;
        }
        static FadeEvent e;
        public static void Trigger(float duration, float targetAlpha)
        {
            e.Duration = duration;
            e.TargetAlpha = targetAlpha;
            NUEventManager.TriggerEvent(e);
        }
    }

    public struct FadeInEvent
    {
        public float Duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrendonBanville.MMInterface.MMFadeInEvent"/> struct.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public FadeInEvent(float duration)
        {
            Duration = duration;
        }
        static FadeInEvent e;
        public static void Trigger(float duration)
        {
            e.Duration = duration;
            NUEventManager.TriggerEvent(e);
        }
    }

    public struct FadeOutEvent
    {
        public float Duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrendonBanville.MMInterface.MMFadeOutEvent"/> struct.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public FadeOutEvent(float duration)
        {
            Duration = duration;
        }

        static FadeOutEvent e;
        public static void Trigger(float duration)
        {
            e.Duration = duration;
            NUEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// The Fader class can be put on an Image, and it'll intercept MMFadeEvents and turn itself on or off accordingly.
    /// </summary>
    public class Fader : MonoBehaviour, NUEventListener<FadeEvent>, NUEventListener<FadeInEvent>, NUEventListener<FadeOutEvent>
    {
        public float InactiveAlpha = 0f;
        public float ActiveAlpha = 1f;

        /// the default duration of the fade in/out
        public float DefaultDuration = 0.2f;
        /// whether or not the fade should happen in unscaled time 
        public bool Unscaled = true;

        protected CanvasGroup _canvasGroup;
        protected Image _image;

        protected float _currentTargetAlpha;
        protected float _currentDuration;

        protected bool _fading = false;
        protected float _fadeDirection = 1f;
        protected float _fadeCounter;

        /// <summary>
        /// On Start, we initialize our fader
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        /// On init, we grab our components, and disable/hide everything
        /// </summary>
        protected virtual void Initialization()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = InactiveAlpha;

            _image = GetComponent<Image>();
            _image.enabled = false;
        }

        /// <summary>
        /// On Update, we update our alpha 
        /// </summary>
        protected virtual void Update()
        {
            if (_canvasGroup == null) { return; }

            if ((_fading) && (_canvasGroup.alpha != _currentTargetAlpha))
            {
                if (_fadeCounter < 1f)
                {
                    EnableFader();

                    _canvasGroup.alpha = Mathf.SmoothStep(_canvasGroup.alpha, _currentTargetAlpha, _fadeCounter);

                    if (Unscaled)
                    {
                        _fadeCounter += Time.unscaledDeltaTime / _currentDuration;
                    }
                    else
                    {
                        _fadeCounter += Time.deltaTime / _currentDuration;
                    }
                }
                else
                {
                    StopFading();
                }
            }
            else
            {
                StopFading();
            }
        }

        /// <summary>
        /// Stops the fading.
        /// </summary>
        protected virtual void StopFading()
        {
            _canvasGroup.alpha = _currentTargetAlpha;
            _fading = false;
            if (_canvasGroup.alpha == InactiveAlpha)
            {
                DisableFader();
            }
        }

        /// <summary>
        /// Disables the fader.
        /// </summary>
        protected virtual void DisableFader()
        {
            _image.enabled = false;
            _canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Enables the fader.
        /// </summary>
        protected virtual void EnableFader()
        {
            _image.enabled = true;
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// When catching a fade event, we fade our image in or out
        /// </summary>
        /// <param name="fadeEvent">Fade event.</param>
        public virtual void OnNUEvent(FadeEvent fadeEvent)
        {
            _fading = true;
            _fadeCounter = 0f;
            _currentTargetAlpha = (fadeEvent.TargetAlpha == -1) ? ActiveAlpha : fadeEvent.TargetAlpha;
            _currentDuration = fadeEvent.Duration;
        }

        /// <summary>
        /// When catching an MMFadeInEvent, we fade our image in
        /// </summary>
        /// <param name="fadeEvent">Fade event.</param>
        public virtual void OnNUEvent(FadeInEvent fadeEvent)
        {
            _fading = true;
            _fadeCounter = 0f;
            _currentTargetAlpha = ActiveAlpha;
            _currentDuration = DefaultDuration;
        }

        /// <summary>
        /// When catching an MMFadeOutEvent, we fade our image out
        /// </summary>
        /// <param name="fadeEvent">Fade event.</param>
        public virtual void OnNUEvent(FadeOutEvent fadeEvent)
        {
            _fading = true;
            _fadeCounter = 0f;
            _currentTargetAlpha = InactiveAlpha;
            _currentDuration = DefaultDuration;
        }

        /// <summary>
        /// On enable, we start listening to events
        /// </summary>
        protected virtual void OnEnable()
        {
            this.NUEventStartListening<FadeEvent>();
            this.NUEventStartListening<FadeInEvent>();
            this.NUEventStartListening<FadeOutEvent>();
        }

        /// <summary>
        /// On disable, we stop listening to events
        /// </summary>
        protected virtual void OnDisable()
        {
            this.NUEventStopListening<FadeEvent>();
            this.NUEventStopListening<FadeInEvent>();
            this.NUEventStopListening<FadeOutEvent>();
        }
    }
}