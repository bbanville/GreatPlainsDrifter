using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace BrendonBanville.Tools
{
    /// <summary>
    /// This class is to be used from other classes, to act as a center point for various feedbacks. 
    /// It's meant to help setup and trigger feedbacks such as vfx, sounds, camera zoom or shake, etc, from an automated entry points in other classes inspectors.
    /// </summary>
	[Serializable]
	public class Feedback
    {
        [Header("Animation")]
        public bool UpdateAnimator;
        [Condition("UpdateAnimator", true)]
        public Animator FeedbackAnimator;
        [Condition("UpdateAnimator", true)]
        public string AnimatorStateName;
        [Condition("UpdateAnimator", true)]
        public string AnimatorTriggerParameterName;
        [Condition("UpdateAnimator", true)]
        public string AnimatorBoolParameterName;
        
        [Header("Particles")]
        /// a particle system already present in your object that will play when the feedback is played, and stopped when it's stopped
        public ParticleSystem Particles;

        [Header("Instantiated VFX")]
        /// whether or not a VFX (or other...) object should be instantiated once when the feedback is played
        public bool InstantiateVFX;
        [Condition("InstantiateVFX", true)]
        /// the vfx object to instantiate
        public GameObject VfxToInstantiate;
        [Condition("InstantiateVFX", true)]
        /// the position offset at which to instantiate the vfx object
        public Vector3 VfxPositionOffset;
        [Condition("InstantiateVFX", true)]
        /// whether or not we should create automatically an object pool for this vfx
        public bool VfxCreateObjectPool;
        [Condition("InstantiateVFX", true)]
        /// the initial and planned size of this object pool
        public int VfxObjectPoolSize = 5;

        [Header("Flicker")]
        public bool FlickerRenderer;
        /// whether or not the renderer should flicker when playing this feedback
        [Condition("FlickerRenderer", true)]
        public Renderer FlickeringRenderer;
        /// the duration of the flicker when getting damage
        [Condition("FlickerRenderer", true)]
        public float FlickerDuration = 0.2f;
        /// the frequency at which to flicker
        [Condition("FlickerRenderer", true)]
        public float FlickerOctave = 0.04f;
        /// the color we should flicker the sprite to 
        [Condition("FlickerRenderer", true)]
        public Color FlickerColor = new Color32(255, 20, 20, 255);

        [Header("Sounds")]
        /// a sound fx to play when this feedback is played
        public AudioClip Sfx;

        [Header("Camera Shake")]
        /// whether or not the camera should shake
        public bool CameraShake = false;
        [Condition("CameraShake", true)]
        public float ShakeMagntitude = 4.0f;
        [Condition("CameraShake", true)]
        public float ShakeRoughness = 4.0f;
        [Condition("CameraShake", true)]
        public float ShakeFadeInTime = 0.1f;
        [Condition("CameraShake", true)]
        public float ShakeFadeOutTime = 1.0f;

        [Header("Freeze Frame")]
        /// whether or not we should freeze the frame when that feedback is played
        public bool FreezeFrame = false;
        [Condition("FreezeFrame", true)]
        /// the duration of the freeze frame
        public float FreezeFrameDuration;

        [Header("Timescale Modification")]
        /// whether or not we should modify the timescale when this feedback is played
        public bool ModifyTimescale = false;
        [Condition("ModifyTimescale", true)]
        /// the new timescale to apply
        public float TimeScale;
        [Condition("ModifyTimescale", true)]
        /// the duration of the timescale modification
        public float TimeScaleDuration;
        [Condition("ModifyTimescale", true)]
        /// whether or not we should lerp the timescale
        public bool TimeScaleLerp;
        [Condition("ModifyTimescale", true)]
        /// the speed at which to lerp the timescale
        public float TimeScaleLerpSpeed;

        [Header("Flash")]
        /// whether or not we should trigger a flash when this feedback is played
        public bool TriggerFlash;

        [Condition("TriggerFlash", false)]
        /// the image to flash
        public Image FlashImage;
        [Condition("TriggerFlash", true)]
        /// the flash duration (in seconds)
        public float FlashDuration = 0.2f;
        [Condition("TriggerFlash", true)]
        /// the alpha of the flash
        public float FlashAlpha = 1f;
        [Condition("TriggerFlash", true)]
        /// the ID of the flash (usually 0). You can specify on each Flash object an ID, allowing you to have different flash images in one scene and call them separately (one for damage, one for health pickups, etc)
        public int FlashID = 0;

        protected SimpleObjectPooler _objectPool;
        protected GameObject _newGameObject;
        protected Color _initialFlickerColor;

        /// <summary>
        /// This method needs to be called by the parent class to initialize the various feedbacks
        /// </summary>
        public virtual void Initialization(GameObject gameObject = null)
        {
            if (InstantiateVFX && VfxCreateObjectPool)
            {
                GameObject objectPoolGo = new GameObject();
                objectPoolGo.name = "FeedbackObjectPool";
                _objectPool = objectPoolGo.AddComponent<SimpleObjectPooler>();
                _objectPool.GameObjectToPool = VfxToInstantiate;
                _objectPool.PoolSize = VfxObjectPoolSize;
                _objectPool.FillObjectPool();
            }
            if (FlickerRenderer && (FlickeringRenderer != null))
            {
                if(FlickeringRenderer.material.HasProperty("_Color"))
                {
                    _initialFlickerColor = FlickeringRenderer.material.color;
                }
            }
            if (FlickerRenderer && (FlickeringRenderer == null) && (gameObject != null))
            {
                if (gameObject.GetComponentNoAlloc<Renderer>() != null)
                {
                    FlickeringRenderer = gameObject.GetComponent<Renderer>();
                }
                if (FlickeringRenderer == null)
                {
                    FlickeringRenderer = gameObject.GetComponentInChildren<Renderer>();
                }
                if (FlickeringRenderer != null)
                {
                    if (FlickeringRenderer.material.HasProperty("_Color"))
                    {
                        _initialFlickerColor = FlickeringRenderer.material.color;
                    }
                }
            }            
        }

        public virtual void ResetFlickerColor()
        {
            if (FlickerRenderer && (FlickeringRenderer != null))
            {
                if (FlickeringRenderer.material.HasProperty("_Color"))
                {
                    FlickeringRenderer.material.color = _initialFlickerColor;
                }
            }
        }

        /// <summary>
        /// Plays all the feedbacks that were enabled for this
        /// </summary>
        /// <param name="position"></param>
        /// <param name="monobehaviour"></param>
        public virtual void Play(Vector3 position, MonoBehaviour monobehaviour = null)
        {
            // Flickering
            if (FlickerRenderer && (FlickeringRenderer != null) && (monobehaviour != null))
            {
                monobehaviour.StartCoroutine(NUImage.Flicker(FlickeringRenderer, _initialFlickerColor, FlickerColor, FlickerOctave, FlickerDuration));
            }

            // Camera shake
            if (CameraShake)
            {
                CameraExtensions.CameraShaker.Instance.ShakeOnce(ShakeMagntitude, ShakeRoughness, ShakeFadeInTime, ShakeFadeOutTime);
            }

            if (UpdateAnimator)
            {
                if (AnimatorStateName != null)
                {
                    FeedbackAnimator.Play(AnimatorStateName);
                }

                if (AnimatorTriggerParameterName != null)
                {
                    FeedbackAnimator.SetTrigger(AnimatorTriggerParameterName);
                }

                if (AnimatorBoolParameterName != null)
                {
                    FeedbackAnimator.SetBool(AnimatorBoolParameterName, true);
                }
            }

            // Instantiated particles
            if (InstantiateVFX && VfxToInstantiate != null)
            {
                if (_objectPool != null)
                {
                    _newGameObject = _objectPool.GetPooledGameObject();
                    if (_newGameObject != null)
                    {
                        _newGameObject.transform.position = position + VfxPositionOffset;
                        _newGameObject.SetActive(true);
                    }
                }
                else
                {
                    _newGameObject = GameObject.Instantiate(VfxToInstantiate) as GameObject;
                    _newGameObject.transform.position = position + VfxPositionOffset;
                }
            }

            // Freeze Frame
            if (FreezeFrame)
            {
                NUFreezeFrameEvent.Trigger(FreezeFrameDuration);
            }

            // Time Scale
            if (ModifyTimescale)
            {
                NUTimeScaleEvent.Trigger(NUTimeScaleMethods.For, TimeScale, TimeScaleDuration, TimeScaleLerp, TimeScaleLerpSpeed, false);
            }

            // Particles
            if (Particles != null)
            {
                Particles.Play();
            }
            
            // Sounds
            if (Sfx != null)
            {
                NUSfxEvent.Trigger(Sfx);
            }
            
            // Flash
            if (TriggerFlash)
            {
                NUFlashEvent.Trigger(FlashImage, FlashDuration, FlashAlpha, FlashID);
            }

        }

        /// <summary>
        /// Stops all the feedbacks that need stopping
        /// </summary>
        public virtual void Stop()
		{
            // Particles
            if (Particles != null)
			{
				Particles.Stop();
			}

            if (UpdateAnimator)
            {
                if (AnimatorBoolParameterName != null)
                {
                    FeedbackAnimator.SetBool(AnimatorBoolParameterName, false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monoBehaviour"></param>
        public void Flicker(MonoBehaviour monoBehaviour = null)
        {
            if (FlickerRenderer && (FlickeringRenderer != null) && (monoBehaviour != null))
            {
                monoBehaviour.StartCoroutine(NUImage.Flicker(FlickeringRenderer, _initialFlickerColor, FlickerColor, FlickerOctave, FlickerDuration));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void TriggerFreezeFrame()
        {
            NUFreezeFrameEvent.Trigger(FreezeFrameDuration);
        }

        /// <summary>
        /// Shakes the camera based on the set feedback magnitude, roughness, fadeintime, and fadeouttime
        /// </summary>
        public void ShakeCamera()
        {
            CameraExtensions.CameraShaker.Instance.ShakeOnce(ShakeMagntitude, ShakeRoughness, ShakeFadeInTime, ShakeFadeOutTime);
        }
        /// <summary>
        /// Shakes the camera based on a given preset CameraShakeInstance
        /// </summary>
        /// <param name="camShake"></param>
        public void ShakeCamera(CameraExtensions.CameraShakeInstance camShake)
        {
            CameraExtensions.CameraShaker.Instance.Shake(camShake);
        }
        /// <summary>
        /// Shakes the camera based on the new inputed override magnitude, roughness, fadeintime, and fadeouttime
        /// </summary>
        /// <param name="shakeMagntitude"></param>
        /// <param name="shakeRoughness"></param>
        /// <param name="shakeFadeInTime"></param>
        /// <param name="shakeFadeOutTime"></param>
        public void ShakeCamera(float shakeMagntitude, float shakeRoughness, float shakeFadeInTime, float shakeFadeOutTime)
        {
            CameraExtensions.CameraShaker.Instance.ShakeOnce(shakeMagntitude, shakeRoughness, shakeFadeInTime, shakeFadeOutTime);
        }
    }
}
