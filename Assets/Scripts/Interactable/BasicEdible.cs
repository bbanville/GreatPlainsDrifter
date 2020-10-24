/*******************************************************************************
File: BasicEdible.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/18/2019
Course: CS176
Section: A
Description:
    The code for the edible interactable.
*******************************************************************************/

using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Environment.Interactable
{
    public class BasicEdible : InteractableObject, IInteractable
    {
        public enum edibleTypes { Alchohol, Food }
        public edibleTypes edibleType = edibleTypes.Alchohol;
        public bool IsConsumed;

        public AudioClip Error;
        public AudioClip Use;

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        /// <summary>
        /// What happens when the player interacts with this object
        /// </summary>
        public void Interact()
        {
            if (CanUse() && hasPlayer)
            {
                if (edibleType == edibleTypes.Alchohol)
                {
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<drunk>().enabled = true;
                }

                if (Use != null)
                {
                    AudioSource.PlayClipAtPoint(Use, transform.position);
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
        private bool CanUse()
        {
            if (!IsConsumed && hasPlayer)
            {
                return true;
            }

            return false;
        }
    }
}