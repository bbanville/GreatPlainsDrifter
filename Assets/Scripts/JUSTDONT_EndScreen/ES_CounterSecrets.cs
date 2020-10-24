using Assets.Scripts.Environment.Interactable;
using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ES_CounterSecrets : MonoBehaviour
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

        var targetCount = new CountUpToTarget();
        if (GameObject.Find("AlchoholicBeverage").GetComponent<BasicEdible>().IsConsumed)
        {
            targetCount.CountToTarget(GetComponent<TextMeshProUGUI>(), 100, 2.5f, 0, "", "%");
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = 0 + "%";
        }
    }
}
