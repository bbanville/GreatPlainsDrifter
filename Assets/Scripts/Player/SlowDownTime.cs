/*******************************************************************************
File: SlowDownTime.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Sets the timescale to a set value to create a slowed time effect.
*******************************************************************************/

using UnityEngine;
using System.Collections;

public class SlowDownTime : MonoBehaviour
{
    [SerializeField] float slowedTimeScale = 0.65f;

	// Use this for initialization
	void Update ()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Time.timeScale = slowedTimeScale;
        }
        else
            Time.timeScale = 1;
    }	
}
