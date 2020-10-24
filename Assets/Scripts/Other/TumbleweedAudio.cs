using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleweedAudio : MonoBehaviour
{
    public List<AudioClip> _collisionSounds;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (_collisionSounds.Count > 0)
        {
            _audioSource.PlayOneShot(_collisionSounds.Random());
        }
    }
}
