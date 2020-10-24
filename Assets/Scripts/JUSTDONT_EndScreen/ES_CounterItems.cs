using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ES_CounterItems : MonoBehaviour
{
    public AudioClip clipToPlay;
    public AudioSource audioSource;

    void OnEnable()
    {
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }

        var targetCount = new CountUpToTarget();
        targetCount.CountToTarget(GetComponent<TextMeshProUGUI>(), 100, 2.5f, 0, "", "%");
    }
}
