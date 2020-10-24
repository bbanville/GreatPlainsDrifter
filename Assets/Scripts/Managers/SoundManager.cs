using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using BrendonBanville.Tools;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// a class to save sound settings (music on or off, sfx on or off)
    /// </summary>
    [Serializable]
    public class SoundSettings
    {
        public bool MusicOn = true;
        public bool SfxOn = true;

        [Header("Music")]
        /// the music volume
        [Range(0, 1)]
        public float MusicVolume = 0.3f;

        [Header("Sound Effects")]
        /// the sound fx volume
        [Range(0, 1)]
        public float SfxVolume = 1f;
    }

    /// <summary>
    /// a class to contain settings for individual sounds
    /// </summary>
    [System.Serializable]
    public class Sound
    {
        public string Name;
        public AudioClip Clip;
        public AudioMixerGroup Output;
        [Range(0.0f, 1.0f)] public float Volume = 1.0f;
        [Range(0.0f, 3.0f)] public float Pitch = 1.0f;
        public bool PlayOnAwake;
        public bool Loop;
    }

    /// <summary>
    /// This persistent singleton handles sound playing
    /// </summary>
    [AddComponentMenu("TopDown Engine/Managers/Sound Manager")]
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        [Header("Settings")]
        public AudioMixer audioMixer;
        public AudioMixerSnapshot musicOn;
        public AudioMixerSnapshot musicOff;
        public AudioMixerSnapshot Inside;
        public AudioMixerSnapshot Outside;
        public AudioMixerSnapshot InDuel;
        public AudioMixerSnapshot InDuel_LowPass;

        /// the current sound settings 
        public SoundSettings Settings;

        [Header("Pause")]
        /// whether or not Sfx should be muted when the game is paused
        public bool MuteSfxOnPause = true;

        protected const string _saveFolderName = "SaveData/";
        protected const string _saveFileName = "sound.settings";
        protected AudioSource _backgroundMusic;
        protected List<AudioSource> _loopingSounds;

        public float audioTransition = 0.2f;

        public List<Sound> SFX = new List<Sound>();
        public Dictionary<string, AudioSource> AudioSources = new Dictionary<string, AudioSource>();

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < SFX.Count; ++i)
            {
                /// Create an audio source for the given sound
                AudioSource source = gameObject.AddComponent<AudioSource>();

                /// Customize audio source settings
                source.clip = SFX[i].Clip;
                source.outputAudioMixerGroup = SFX[i].Output;
                source.volume = SFX[i].Volume;
                source.pitch = SFX[i].Pitch;
                source.playOnAwake = SFX[i].PlayOnAwake;
                source.loop = SFX[i].Loop;

                /// Save audiosource into dictionary
                AudioSources.Add(SFX[i].Name, source);
            }
        }

        /// <summary>
        /// Plays a background music.
        /// Only one background music can be active at a time.
        /// </summary>
        /// <param name="Clip">Your audio clip.</param>
        public virtual void PlayBackgroundMusic(AudioSource Music)
        {
            // if the music's been turned off, we do nothing and exit
            if (!Settings.MusicOn)
                return;
            // if we already had a background music playing, we stop it
            if (_backgroundMusic != null)
                _backgroundMusic.Stop();
            // we set the background music clip
            _backgroundMusic = Music;
            // we set the music's volume
            _backgroundMusic.volume = Settings.MusicVolume;
            // we set the loop setting to true, the music will loop forever
            _backgroundMusic.loop = true;
            // we start playing the background music
            _backgroundMusic.Play();
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <returns>An audiosource</returns>
        /// <param name="sfx">The sound clip you want to play.</param>
        /// <param name="location">The location of the sound.</param>
        /// <param name="loop">If set to true, the sound will loop.</param>
        public virtual AudioSource PlaySound(AudioClip sfx, Vector3 location, bool loop = false)
        {
            if (!Settings.SfxOn)
                return null;
            // we create a temporary game object to host our audio source
            GameObject temporaryAudioHost = new GameObject("TempAudio");
            // we set the temp audio's position
            temporaryAudioHost.transform.position = location;
            // we add an audio source to that host
            AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
            // we set that audio source clip to the one in paramaters
            audioSource.clip = sfx;
            // we set the audio source volume to the one in parameters
            audioSource.volume = Settings.SfxVolume;
            // we set our loop setting
            audioSource.loop = loop;
            // we start playing the sound
            audioSource.Play();

            if (!loop)
            {
                // we destroy the host after the clip has played
                Destroy(temporaryAudioHost, sfx.length);
            }
            else
            {
                _loopingSounds.Add(audioSource);
            }

            // we return the audiosource reference
            return audioSource;
        }


        /// Plays a sound clip globally from a generated audio source
        public void PlaySoundFromDict(string soundName)
        {
            AudioSource source;
            var sourceExists = AudioSources.TryGetValue(soundName, out source);
            if (sourceExists)
            {
                source.Play();
            }
            else
            {
                Debug.LogWarning("No sound with name " + soundName + " was found");
            }
        }

        /// <summary>
        /// Stops the looping sounds if there are any
        /// </summary>
        /// <param name="source">Source.</param>
        public virtual void StopLoopingSound(AudioSource source)
        {
            if (source != null)
            {
                _loopingSounds.Remove(source);
                Destroy(source.gameObject);
            }
        }

        /// <summary>
        /// Sets the music on/off setting based on the value in parameters
        /// This value will be saved, and any music played after that setting change will comply
        /// </summary>
        /// <param name="status"></param>
		protected virtual void MuteUnmuteMusic(bool status)
        {
            Settings.MusicOn = status;
            SaveSoundSettings();
            if (status)
            {
                UnmuteBackgroundMusic();
            }
            else
            {
                MuteBackgroundMusic();
            }
        }

        /// <summary>
        /// Sets the SFX on/off setting based on the value in parameters
        /// This value will be saved, and any SFX played after that setting change will comply
        /// </summary>
        /// <param name="status"></param>
		protected virtual void MuteUnmuteSfx(bool status)
        {
            Settings.SfxOn = status;
            SaveSoundSettings();
        }

        /// <summary>
        /// Sets the music setting to On
        /// </summary>
		public virtual void MusicOn() { MuteUnmuteMusic(true); }

        /// <summary>
        /// Sets the Music setting to Off
        /// </summary>
		public virtual void MusicOff() { MuteUnmuteMusic(false); }

        /// <summary>
        /// Sets the SFX setting to On
        /// </summary>
		public virtual void SfxOn() { MuteUnmuteSfx(true); }

        /// <summary>
        /// Sets the SFX setting to Off
        /// </summary>
		public virtual void SfxOff() { MuteUnmuteSfx(false); }

        /// <summary>
        /// Saves the sound settings to file
        /// </summary>
		protected virtual void SaveSoundSettings()
        {
            SaveLoadManager.Save(Settings, _saveFileName, _saveFolderName);
        }

        /// <summary>
        /// Loads the sound settings from file (if found)
        /// </summary>
		protected virtual void LoadSoundSettings()
        {
            SoundSettings settings = (SoundSettings)SaveLoadManager.Load(_saveFileName, _saveFolderName);
            if (settings != null)
            {
                Settings = settings;
            }
        }

        /// <summary>
        /// Resets the sound settings by destroying the save file
        /// </summary>
		protected virtual void ResetSoundSettings()
        {
            SaveLoadManager.DeleteSave(_saveFileName, _saveFolderName);
        }

        /// <summary>
        /// Mutes all sfx currently playing
        /// </summary>
        protected virtual void MuteAllSfx()
        {
            foreach (AudioSource source in _loopingSounds)
            {
                if (source != null)
                {
                    source.mute = true;
                }
            }
        }

        /// <summary>
        /// Unmutes all sfx currently playing
        /// </summary>
		protected virtual void UnmuteAllSfx()
        {
            foreach (AudioSource source in _loopingSounds)
            {
                if (source != null)
                {
                    source.mute = false;
                }
            }
        }

        /// <summary>
        /// Unmutes the background music
        /// </summary>
        public virtual void UnmuteBackgroundMusic()
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.mute = false;
            }

            if (audioMixer != null)
            {
                //audioMixer.SetFloat("musicVol", 0);
            }
        }

        /// <summary>
        /// Mutes the background music
        /// </summary>
        public virtual void MuteBackgroundMusic()
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.mute = true;
            }

            if (audioMixer != null)
            {
            }
        }

        public float GetMusicVol()
        {
            return Settings.MusicVolume;
        }

        public float GetSFXVol()
        {
            return Settings.SfxVolume;
        }

        public void SetMusicVol(float newVol)
        {
            Settings.MusicVolume = newVol;
            audioMixer.SetFloat("musicVol", newVol);
        }

        public void SetSFXVol(float newVol)
        {
            Settings.SfxVolume = newVol;
            audioMixer.SetFloat("sfxVol", ((newVol * 100) - 70));
            //audioMixer.SetFloat("sfxVol", newVol);
        }

        /// <summary>
        /// On enable we start listening for events
        /// </summary>
        protected virtual void OnEnable()
        {
            LoadSoundSettings();
            _loopingSounds = new List<AudioSource>();
        }

        /// <summary>
        /// On disable we stop listening for events
        /// </summary>
		protected virtual void OnDisable()
        {
            if (_enabled)
            {

            }
        }
    }

    [System.Serializable]
    public class AudioState
    {
        public string Name;
        public AudioMixerSnapshot Snapshot;
    }
}