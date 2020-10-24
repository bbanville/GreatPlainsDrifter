using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ES_CounterTime : MonoBehaviour
{
    public AudioClip clipToPlay;
    public EndScreen endScreen;
    public AudioSource audioSource;

    void OnEnable()
    {
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }

        TimeSpan time = TimeSpan.FromSeconds(endScreen.NextBestTime);
        GetComponent<TextMeshProUGUI>().text = time.ToString("mm':'ss");
    }
}
