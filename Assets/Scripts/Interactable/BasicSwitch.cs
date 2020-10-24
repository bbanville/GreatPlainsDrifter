/*******************************************************************************
File: BasicSwitch.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/18/2019
Course: CS176
Section: A
Description:
    The code for the switch interactble.
*******************************************************************************/

using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Environment.Interactable
{
    public class BasicSwitch : InteractableObject, IInteractable
    {
        public GameObject Switchable;
        public string RequiredKey;
        public bool IsOn;

        public AudioClip Error;
        public AudioClip Open;

        void Start()
        {
            Switchable.SetActive(IsOn);
        }

        void Update()
        {
            
        }

        /// <summary>
        /// What happens when the player interacts with this object
        /// </summary>
        public virtual void Interact()
        {
            if (CanUse())
            {
                IsOn = !IsOn;

                Switchable.SetActive(IsOn);

                if (Open)
                {
                    AudioSource.PlayClipAtPoint(Open, transform.position);
                }
            }
            else
            {
                if (Error)
                {
                    AudioSource.PlayClipAtPoint(Error, transform.position);
                }
            }
        }

        /// <summary>
        /// Checks if the player can pull the switch
        /// </summary>
        /// <returns></returns>
        public virtual bool CanUse()
        {
            if (string.IsNullOrEmpty(RequiredKey) || PlayerInventory.HasItem(RequiredKey))
            {
                return true;
            }

            return false;
        }
    }
}