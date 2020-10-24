using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using BrendonBanville.Tools;

public class PlayAudioAtPoint : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;
    [ReadOnly] bool Playing;
    SoundManager SM;

    // Start is called before the first frame update
    void Start()
    {
        SM = GameObject.FindGameObjectWithTag("AM").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Playing)
        {
            SM.PlaySound(audioClip, this.transform.position, true);
            Playing = true;
        }
    }
}
