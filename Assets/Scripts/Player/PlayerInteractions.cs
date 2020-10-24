/*******************************************************************************
File: PlayerInteractions.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Allows the player to interact with objects as well as give them a signifier
    to when they may do so.
*******************************************************************************/

using Assets.Scripts.Environment.Interactable;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteractions : MonoBehaviour
{
    public TextMeshProUGUI InteractionText;
    public float InteractionDistance = 5;

    private void Update()
    {
        InteractionText.text = "";

        RaycastHit hit;
        var cam = UnityEngine.Camera.main.transform;
        var forward = cam.forward;
        var position = cam.position;

        /// </summary>
        /// Checks a raycast to see if the player is looking at an object
        /// </summary>
        var ray = new Ray(position, forward);

        if (Physics.Raycast(ray, out hit))
        {
            var interaction = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interaction != null && Vector3.Distance(transform.position, hit.collider.transform.position) < InteractionDistance)
            {
                if (Input.GetButtonDown("Use"))
                {
                    // Triggers an interaction
                    interaction.Interact();
                }
                else
                {
                    // Changes the text to signify a interactable
                    InteractionText.text = "Press E to interact";
                }
            }
        }
    }
}