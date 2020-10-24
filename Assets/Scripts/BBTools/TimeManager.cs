using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BrendonBanville.Tools;

namespace BrendonBanville.Tools
{
	/// <summary>
	/// A list of the methods available to change the current score
	/// </summary>
	public enum NUTimeScaleMethods
	{
		Set,
		For,
		Reset,
        Unfreeze
	}

	/// <summary>
	/// The different settings you can play with on a timescale event
	/// </summary>
	public struct NUTimeScaleProperties
	{
		public float TimeScale;
		public float Duration;
		public bool Lerp;
		public float LerpSpeed;
        public bool Infinite;
	}

	/// <summary>
	/// A time scale event, used to change the current timescale forever or for a certain duration
	/// </summary>
	public struct NUTimeScaleEvent
	{
		public NUTimeScaleMethods TimeScaleMethod;
		public NUTimeScaleProperties TimeScaleProperty;

		public NUTimeScaleEvent(NUTimeScaleMethods timeScaleMethod, float timeScale, float duration, bool lerp, float lerpSpeed, bool infinite)
		{
			TimeScaleMethod = timeScaleMethod;
			TimeScaleProperty.TimeScale = timeScale;
			TimeScaleProperty.Duration = duration;
			TimeScaleProperty.Lerp = lerp;
			TimeScaleProperty.LerpSpeed = lerpSpeed;
            TimeScaleProperty.Infinite = infinite;
		}
        
        static NUTimeScaleEvent e;
        public static void Trigger(NUTimeScaleMethods timeScaleMethod, float timeScale, float duration, bool lerp, float lerpSpeed, bool infinite)
        {
            e.TimeScaleMethod = timeScaleMethod;
            e.TimeScaleProperty.TimeScale = timeScale;
            e.TimeScaleProperty.Duration = duration;
            e.TimeScaleProperty.Lerp = lerp;
            e.TimeScaleProperty.LerpSpeed = lerpSpeed;
            e.TimeScaleProperty.Infinite = infinite;
            NUEventManager.TriggerEvent(e);
        }
    }

	/// <summary>
	/// An event used to freeze the whole screen for a certain duration, and return it back to a certain timescale afterwards
	/// </summary>
	public struct NUFreezeFrameEvent
	{
		public float FreezeDuration;
		public NUFreezeFrameEvent(float duration)
		{
			FreezeDuration = duration;
		}
        
        static NUFreezeFrameEvent e;
        public static void Trigger(float duration)
        {
            e.FreezeDuration = duration;
            NUEventManager.TriggerEvent(e);
        }
    }

	/// <summary>
	/// Put this component in your scene and it'll catch NUFreezeFrameEvents and NUTimeScaleEvents, allowing you to control the flow of time.
	/// </summary>
	public class TimeManager : MonoBehaviour, NUEventListener<NUFreezeFrameEvent>, NUEventListener<NUTimeScaleEvent> 
	{		
		[InformationAttribute("Put this component in your scene and it'll catch NUFreezeFrameEvents and NUTimeScaleEvents, allowing you to control the flow of time.", InformationAttribute.InformationType.Info, false)]
		/// The reference timescale, to which the system will go back to after all time is changed
		public float NormalTimescale = 1f;
		[ReadOnly]
		/// the current, real time, time scale
		public float CurrentTimeScale = 1f;
		[ReadOnly]
		/// the time scale the system is lerping towards
		public float TargetTimeScale = 1f;
		[ReadOnly]
		/// whether or not the timescale should be lerping
		public bool LerpTimescale = true;
		[ReadOnly]
		/// the speed at which the timescale should lerp towards its target
		public float LerpSpeed;

		[InspectorButtonAttribute("TestButtonToSlowDownTime")]
		/// a test button for the inspector
		public bool TestButton;

		protected Stack<NUTimeScaleProperties> _timeScaleProperties;
		protected float _frozenTimeLeft = -1f;
		protected NUTimeScaleProperties _currentProperty;

		/// <summary>
		/// A method used from the inspector to test the system
		/// </summary>
		protected virtual void TestButtonToSlowDownTime()
		{
			NUTimeScaleEvent.Trigger(NUTimeScaleMethods.For, 0.5f, 3f, true, 1f, false);
		}

		/// <summary>
		/// On start we initialize our stack and store our initial time scale
		/// </summary>
		protected virtual void Start()
		{
			TargetTimeScale = NormalTimescale;
			_timeScaleProperties = new Stack<NUTimeScaleProperties>();
		}

		/// <summary>
		/// On Update, applies the timescale and resets it if needed
		/// </summary>
		protected virtual void Update()
		{          

            // if we have things in our stack, we handle them, otherwise we reset to the normal timescale
            if (_timeScaleProperties.Count > 0)
			{
				_currentProperty = _timeScaleProperties.Peek();

				TargetTimeScale = _currentProperty.TimeScale;
				LerpSpeed = _currentProperty.LerpSpeed;
				LerpTimescale = _currentProperty.Lerp;
				_currentProperty.Duration -= Time.unscaledDeltaTime;

				_timeScaleProperties.Pop();
				_timeScaleProperties.Push(_currentProperty);

				if (_currentProperty.Duration <= 0f && !_currentProperty.Infinite)
				{
					Unfreeze();
				}
			}
			else
			{
				TargetTimeScale = NormalTimescale;
			}

			// we apply our timescale
			if (LerpTimescale)
			{
				Time.timeScale = Mathf.Lerp(Time.timeScale, TargetTimeScale, Time.unscaledDeltaTime * LerpSpeed);
			}
			else
			{
				Time.timeScale = TargetTimeScale;
			}

			CurrentTimeScale = Time.timeScale;
		}

		/// <summary>
		/// Resets all stacked timescale changes and simply sets the timescale, until further changes
		/// </summary>
		/// <param name="newTimeScale">New time scale.</param>
		protected virtual void SetTimeScale(float newTimeScale)
		{
			_timeScaleProperties.Clear();
			Time.timeScale = newTimeScale;
		}

		/// <summary>
		/// Sets the time scale for the specified properties (duration, time scale, lerp or not, and lerp speed)
		/// </summary>
		/// <param name="timeScaleProperties">Time scale properties.</param>
		protected virtual void SetTimeScale(NUTimeScaleProperties timeScaleProperties)
		{
			_timeScaleProperties.Push(timeScaleProperties);
		}

		/// <summary>
		/// Resets the time scale to the stored normal timescale
		/// </summary>
		protected virtual void ResetTimeScale()
		{
			Time.timeScale = NormalTimescale;
		}

		/// <summary>
		/// Resets the time scale to the last saved time scale.
		/// </summary>
		protected virtual void Unfreeze()
        {
            if (_timeScaleProperties.Count > 0)
			{
                _timeScaleProperties.Pop();
			}
            else
            {
                ResetTimeScale();
            }
		}

		/// <summary>
		/// Sets the normal time scale.
		/// </summary>
		/// <param name="newNormalTimeScale">New normal time scale.</param>
		public virtual void SetNormalTimeScale(float newNormalTimeScale)
		{
			NormalTimescale = newNormalTimeScale;
		}

		/// <summary>
		/// Catches TimeScaleEvents and acts on them
		/// </summary>
		/// <param name="timeScaleEvent">MMTimeScaleEvent event.</param>
		public virtual void OnNUEvent(NUTimeScaleEvent timeScaleEvent)
		{
			switch (timeScaleEvent.TimeScaleMethod)
			{
				case NUTimeScaleMethods.Reset:
					ResetTimeScale ();
					break;

				case NUTimeScaleMethods.Set:
					SetTimeScale (timeScaleEvent.TimeScaleProperty.TimeScale);
					break;

				case NUTimeScaleMethods.For:
					SetTimeScale (timeScaleEvent.TimeScaleProperty);
					break;

                case NUTimeScaleMethods.Unfreeze:
                    Unfreeze();
                    break;
			}
		}

		/// <summary>
		/// When getting a freeze frame event we stop the time
		/// </summary>
		/// <param name="freezeFrameEvent">Freeze frame event.</param>
		public void OnNUEvent(NUFreezeFrameEvent freezeFrameEvent)
		{
			_frozenTimeLeft = freezeFrameEvent.FreezeDuration;

			NUTimeScaleProperties properties = new NUTimeScaleProperties();
			properties.Duration = freezeFrameEvent.FreezeDuration;
			properties.Lerp = false;
			properties.LerpSpeed = 0f;
			properties.TimeScale = 0f;

			SetTimeScale(properties);
		} 

		/// <summary>
		/// On enable, starts listening for FreezeFrame events
		/// </summary>
		void OnEnable()
		{
			this.NUEventStartListening<NUFreezeFrameEvent>();
			this.NUEventStartListening<NUTimeScaleEvent>();
		}

		/// <summary>
		/// On disable, stops listening for FreezeFrame events
		/// </summary>
		void OnDisable()
		{
			this.NUEventStopListening<NUFreezeFrameEvent>();
			this.NUEventStopListening<NUTimeScaleEvent>();
		}		
	}

    [System.Serializable]
    public class TimePoint
    {
        public string timePointName;
        public int minutes;
        public int seconds;
        public int milliseconds;
    }
}
