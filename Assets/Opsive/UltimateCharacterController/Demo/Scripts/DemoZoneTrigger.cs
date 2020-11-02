/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Utility;
using System;
//using Opsive.UltimateCharacterController.Events;

namespace Opsive.UltimateCharacterController.Demo
{
    /// <summary>
    /// Notifies the DemoManager when the character enters a trigger.
    /// </summary>
    public class DemoZoneTrigger : MonoBehaviour
    {
        private DemoManager m_DemoManager;
        //public EventHandler OnPlayerTriggerEnter;
        public event EventHandler OnPlayerTriggerEnter;
        //public EventHandler OnPlayerTriggerExit;
        public event EventHandler OnPlayerTriggerExit;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_DemoManager = GetComponentInParent<DemoManager>();
        }

        /// <summary>
        /// An object has entered the trigger.
        /// </summary>
        /// <param name="other">The object that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            // A main character collider is required.
            if (!MathUtility.InLayerMask(other.gameObject.layer, 1 << LayerManager.Character))
            {
                return;
            }

            OnPlayerTriggerEnter?.Invoke(this, EventArgs.Empty);
            m_DemoManager.EnteredTriggerZone(this, other.gameObject);
        }

        /// <summary>
        /// An object has exited the trigger.
        /// </summary>
        /// <param name="other">The collider that exited the trigger.</param>
        private void OnTriggerExit(Collider other)
        {
            // A main character collider is required.
            if (!MathUtility.InLayerMask(other.gameObject.layer, 1 << LayerManager.Character))
            {
                return;
            }

            if (other.gameObject.GetCachedParentComponent<UltimateCharacterLocomotion>() == null || other.gameObject.GetCachedParentComponent<LocalLookSource>() != null)
            {
                return;
            }

            OnPlayerTriggerExit?.Invoke(this, EventArgs.Empty);
            m_DemoManager.ExitedTriggerZone(this);
        }
    }
}