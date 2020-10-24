﻿/*******************************************************************************
File: Bobber.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Sets the player height and bobs the player camera according to a given value.
*******************************************************************************/

using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Bobber : MonoBehaviour
    {
        public float BobbingSpeed = 0.18f;
        public float BobbingAmount = 0.1f;
        public float HeadHeight = 0.0f;

        private float _timer;
        private readonly float _floatTolerance = .01f;

        void Update()
        {
            var waveslice = 0.0f;
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var newHeadPosition = transform.localPosition;

            if (Math.Abs(Mathf.Abs(horizontal)) < _floatTolerance && Math.Abs(Mathf.Abs(vertical)) < _floatTolerance)
            {
                _timer = 0.0f;
            }
            else
            {
                waveslice = Mathf.Sin(_timer);
                _timer = _timer + BobbingSpeed;
                if (_timer > Mathf.PI * 2)
                {
                    _timer = _timer - (Mathf.PI * 2);
                }
            }

            if (Math.Abs(waveslice) > _floatTolerance)
            {
                var translateChange = waveslice * BobbingAmount;
                var totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;
                newHeadPosition.y = HeadHeight + translateChange;
            }
            else
            {
                newHeadPosition.y = HeadHeight;
            }

            transform.localPosition = newHeadPosition;
        }
    }
}