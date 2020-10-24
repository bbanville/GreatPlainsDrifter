/*******************************************************************************
File: BasicDoor.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/18/2019
Course: CS176
Section: A
Description:
    The code for the door interactble.
*******************************************************************************/

using Assets.Scripts.Player;
using BrendonBanville.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Environment.Interactable
{
    public class BasicDoor : MonoBehaviour, IInteractable
    {
        public float Speed = .01f;
        public float MoveAmount = 1.1f;
        public string RequiredKey;
        public AudioClip Error;
        public List<AudioClip> Open;
        public List<AudioClip> Close;

        private bool _isOpen;
        private bool _wasUsed;
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        void Start()
        {
            
        }

        void Update()
        {
            if (_wasUsed)
            {
                transform.position = Vector3.MoveTowards(transform.position, _endPosition, Speed);
            }
        }

        /// <summary>
        /// What happens when the player interacts with this object
        /// </summary>
        public void Interact()
        {
            if (CanUse())
            {
                if (!_wasUsed)
                {
                    _wasUsed = true;
                    _startPosition = transform.position;
                    _endPosition = _startPosition + transform.right * MoveAmount;

                    if (Open.Count > 0 && !_isOpen)
                    {
                        AudioSource.PlayClipAtPoint(Open.Random(), transform.position);
                    }
                    else if (Close.Count > 0 && _isOpen)
                    {
                        AudioSource.PlayClipAtPoint(Close.Random(), transform.position);
                    }
                }
            }
            else
            {
                AudioSource.PlayClipAtPoint(Error, transform.position);
            }
        }

        /// <summary>
        /// Checks if the player can open the door
        /// </summary>
        /// <returns></returns>
        private bool CanUse()
        {
            if (string.IsNullOrEmpty(RequiredKey) || PlayerInventory.HasItem(RequiredKey))
            {
                return true;
            }

            return false;
        }
    }
}