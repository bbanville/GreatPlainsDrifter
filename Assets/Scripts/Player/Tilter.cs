/*******************************************************************************
File: Tilter.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Tilts the player camera horizontally by a given value, when the player
    moves in that direction.
*******************************************************************************/

using BrendonBanville.Tools;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Tilter : MonoBehaviour
    {
        public float MaxTilt = 2;
        public float TiltSpeed = 1f;

        private float _currentTilt;
        
        void Update()
        {
            var sideMovemnt = Input.GetAxisRaw("Horizontal");
            var tilt = -1 * sideMovemnt * MaxTilt;

            _currentTilt = _currentTilt.MoveTowards(tilt, TiltSpeed);

            var rot = transform.localRotation;
            transform.localRotation = Quaternion.AngleAxis(_currentTilt, Vector3.forward);
        }
    }
}