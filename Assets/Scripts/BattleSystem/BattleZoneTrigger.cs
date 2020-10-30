
using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Utility;
using System;

/// <summary>
/// Notifies the BattleSystem when the character enters a trigger.
/// </summary>
public class BattleZoneTrigger : MonoBehaviour
{
    public event EventHandler OnPlayerTriggerEnter;
    public event EventHandler OnPlayerTriggerExit;

    private LevelManager m_LevelManager;

    /// <summary>
    /// Initialize the default values.
    /// </summary>
    private void Awake()
    {
        m_LevelManager = GetComponentInParent<LevelManager>();
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
        m_LevelManager.EnteredTriggerZone(this, other.gameObject);
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
        m_LevelManager.ExitedTriggerZone(this);
    }
}