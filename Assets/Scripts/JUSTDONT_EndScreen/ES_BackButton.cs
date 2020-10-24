using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_BackButton : MonoBehaviour
{
    public AudioClip clipToPlay;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    void OnEnable()
    {
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}
