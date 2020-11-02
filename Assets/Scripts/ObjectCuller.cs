using Opsive.UltimateCharacterController.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCuller : MonoBehaviour
{
    [SerializeField] protected bool setActiveOnExit;
    [SerializeField] protected List<GameObject> cullingGroup;

    [Tooltip("A reference to the character.")]
    [SerializeField] protected GameObject m_Character;

    /// <summary>
    /// Initializes the character.
    /// </summary>
    protected void Start()
    {
        if (m_Character == null)
        {
            m_Character = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_Character.tag || other.tag == "Player")
        {
            foreach (GameObject gameObject in cullingGroup)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (setActiveOnExit)
        {
            if (other.tag == m_Character.tag || other.tag == "Player")
            {
                foreach (GameObject gameObject in cullingGroup)
                {
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
