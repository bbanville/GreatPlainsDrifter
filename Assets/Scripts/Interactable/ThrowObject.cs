/*******************************************************************************
File: ThrowObject.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Turns the object it is attached to into a throwable object, that can be
    picked up, dropped, and thrown.
*******************************************************************************/

using UnityEngine;
using Assets.Scripts.Environment.Interactable;
using System.Collections;
using Assets.Scripts.Player;

public class ThrowObject : MonoBehaviour, IInteractable
{
    private Transform playerCam;
    public float throwForce = 1500.0f;
    bool beingCarried = false;
    public int dmg;
    private bool touched = false;
    private Transform player;
    bool hasPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

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

        if (Input.GetButtonDown("Use") && hasPlayer)
        {
            Interact();
        }

        /// </summary>
        /// Checks what to do with the item based upon an inputed value
        /// </summary>
        if (beingCarried)
        {
            if (touched)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                touched = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                GetComponent<Rigidbody>().AddForce(playerCam.forward * throwForce);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
            }
        }
    }

    public void Interact()
    {
        if (hasPlayer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = playerCam;
            beingCarried = true;
        }
    }

    void OnTriggerEnter()
    {
        if (beingCarried)
        {
            touched = true;
        }
    }
}