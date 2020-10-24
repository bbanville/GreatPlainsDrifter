using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSound : MonoBehaviour
{
    public List<AudioClip> sounds;
    
    private AudioSource _audioSource;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void PlaySound()
    {
        if (sounds.Count > 0)
        {
            _audioSource.PlayOneShot(sounds.Random());
        }
    }
}
