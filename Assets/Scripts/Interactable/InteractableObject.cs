/*******************************************************************************
File: InteractableObject.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Contains shared base for all interactable objects.
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts.Player;

namespace Assets.Scripts.Environment.Interactable
{
    public class InteractableObject : MonoBehaviour
    {
        private Transform player;
        public bool hasPlayer = false;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update()
        {
            /// </summary>
            /// Checks if the player is within a given distance from the player
            /// </summary>
            float dist = Vector3.Distance(gameObject.transform.position, player.position);
            if (dist <= player.GetComponentInChildren<PlayerInteractions>().InteractionDistance)
            {
                hasPlayer = true;
            }
            else
            {
                hasPlayer = false;
            }
        }
    }
}
