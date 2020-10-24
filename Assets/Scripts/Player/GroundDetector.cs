/*******************************************************************************
File: GroundDetector.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Detects if the player is touching the ground.
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class GroundDetector : MonoBehaviour
    {
        private int ContactCount = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger) ++ContactCount;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.isTrigger) --ContactCount;
        }

        public bool IsGrounded()
        {
            if (ContactCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
