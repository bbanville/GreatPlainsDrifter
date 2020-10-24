/*******************************************************************************
File: DestroyOnTrigger.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Destroys an item on a trigger and instantiates a feedback effect.
*******************************************************************************/

using UnityEngine;
using System.Collections;

public class DestroyOnTrigger : MonoBehaviour
{
    public GameObject objToDestroy;
    public GameObject effect;

    // Use this for initialization
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Instantiate(effect, objToDestroy.transform.position, objToDestroy.transform.rotation);
            Destroy(objToDestroy);
        }
    }
}