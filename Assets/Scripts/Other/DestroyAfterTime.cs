/*******************************************************************************
File: DestroyAfterTime.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Destroys an item after a set time and instantiates a feedback effect.
*******************************************************************************/

using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{
    public GameObject effect;
    public float Time;

    // Use this for initialization
    void Start()
    {
        Invoke("Explode", 5);
    }

    void Explode()
    {
        if (effect)
        {
            Instantiate(effect, gameObject.transform.position, gameObject.transform.rotation);
        }
        Destroy(gameObject);
    }
}