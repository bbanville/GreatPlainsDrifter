using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Title : MonoBehaviour
{
    public AudioClip clipToPlay;
    public AudioSource audioSource;

    void OnEnable()
    {
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}
