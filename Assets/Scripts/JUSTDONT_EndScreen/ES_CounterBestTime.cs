using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ES_CounterBestTime : MonoBehaviour
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

        TimeSpan time = TimeSpan.FromSeconds(endScreen.BestTime);
        GetComponent<TextMeshProUGUI>().text = time.ToString("mm':'ss");
    }
}