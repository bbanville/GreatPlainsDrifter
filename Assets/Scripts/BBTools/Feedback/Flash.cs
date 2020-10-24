using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BrendonBanville.Tools;

namespace BrendonBanville.Tools
{
	/// <summary>
	/// An event to trigger to flash something for a frame
	/// </summary>
	public struct NUFlashEvent
	{
        /// the image to flash
		public Image FlashImage;
        /// the duration, in seconds, of the flash
        public float FlashDuration;
        /// the max alpha (between 0 and 1) at the peak of the flash
        public float FlashAlpha;
        /// the ID of the MMFlash object to call (specified in its inspector)
        public int FlashID;

		/// <summary>
		/// Initializes a new instance of the <see cref="BrendonBanville.Tools.NUFlashEvent"/> struct.
		/// </summary>
		/// <param name="flashImage">Flash image.</param>
        public NUFlashEvent(Image flashImage, float duration, float alpha, int flashID)
        {
            FlashImage = flashImage;
            FlashDuration = duration;
            FlashAlpha = alpha;
            FlashID = flashID;
        }

        static NUFlashEvent e;
        public static void Trigger(Image flashImage, float duration, float alpha, int flashID)
        {
            e.FlashImage = flashImage;
            e.FlashDuration = duration;
            e.FlashAlpha = alpha;
            e.FlashID = flashID;
            NUEventManager.TriggerEvent(e);
        }
    }

	[RequireComponent(typeof(Image))]
	/// <summary>
	/// Add this class to an image and it'll flash when getting a FlashEvent
	/// </summary>
	public class Flash : MonoBehaviour, NUEventListener<NUFlashEvent> 
	{
        /// the ID of this Flash object. When triggering a FlashEvent you can specify an ID, and only Flash objects with this ID will answer the call and flash, allowing you to have more than one flash object in a scene
        public int FlashID = 0;

		protected Image _image;
        protected CanvasGroup _canvasGroup;
		protected bool _flashing = false;
        protected float _targetAlpha;
        protected Color _initialColor;
        protected float _delta;
        protected float _flashStartedTimestamp;
        protected int _direction = 1;
        protected float _duration;        

		/// <summary>
		/// On start we grab our image component
		/// </summary>
		protected virtual void Start()
		{
			_image = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _initialColor = _image.color;
        }

		/// <summary>
		/// On update we flash our image if needed
		/// </summary>
		protected virtual void Update()
		{
			if (_flashing)
			{
				_image.enabled = true;


                if (Time.time - _flashStartedTimestamp > _duration / 2f)
                {
                    _direction = -1;
                }

                if (_direction == 1)
                {
                    _delta += Time.deltaTime / (_duration / 2f);
                }
                else
                {
                    _delta -= Time.deltaTime / (_duration / 2f);
                }
                
                if (Time.time - _flashStartedTimestamp > _duration)
                {
                    _flashing = false;
                }

                _canvasGroup.alpha = Mathf.Lerp(0f, _targetAlpha, _delta);
            }
			else
			{
				_image.enabled = false;
			}
		}

		/// <summary>
		/// When getting a flash event, we turn our image on
		/// </summary>
		public void OnNUEvent(NUFlashEvent flashEvent)
        {
            if (flashEvent.FlashID != FlashID)
            {
                return;
            }
            if (!_flashing)
            {
                _flashing = true;
                _direction = 1;
                _canvasGroup.alpha = 0;
                _targetAlpha = flashEvent.FlashAlpha;
                _delta = 0f;
                _image = flashEvent.FlashImage;
                _duration = flashEvent.FlashDuration;
                _flashStartedTimestamp = Time.time;
            }
        } 

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.NUEventStartListening<NUFlashEvent>();
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.NUEventStopListening<NUFlashEvent>();
		}		
	}
}