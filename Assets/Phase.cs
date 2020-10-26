using BrendonBanville.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Phase : MonoBehaviour
{
    [Header("System Tracking")]
    public int phaseID = 0;

    [Header("Object Lists")]
    [SerializeField] protected List<GameObject> enemyPhase;
    [SerializeField] protected List<GameObject> objectPhase;

    [ReadOnly] public bool phaseComplete = false;
    [ReadOnly] private bool _PhaseSpawned = false;
    [ReadOnly] private bool _PrevPhaseComplete = false;

    /// <summary>
    /// Awake is called once at the beginning of the scene
    /// </summary>
    protected void Awake()
    {

    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    protected void Update()
    {
        if (!phaseComplete)
        {
            if (phaseID == 0)
            {
                _PrevPhaseComplete = true;
            }
            else
            {
                _PrevPhaseComplete = PhaseManager.Instance.PrevPhaseComplete();
            }

            if (_PrevPhaseComplete)
            {
                if (!_PhaseSpawned)
                {
                    SpawnEnemies();
                    SpawnObjects();
                    _PhaseSpawned = true;
                }

                if (CheckEnemies() == true)
                {
                    phaseComplete = true;
                    Invoke("DespawnEnemies", 2.0f);
                }
            }
        }
        else
        {
            PhaseManager.Instance.SpawnNextPhase();
        }
    }

    /// <summary>
    /// Spawns all enemies in the phase.
    /// </summary>
    public void SpawnEnemies()
    {
        foreach (GameObject enemy in enemyPhase)
        {
            enemy.SetActive(true);
        }
    }

    /// <summary>
    /// Despawns all enemies in the phase.
    /// </summary>
    protected void DespawnEnemies()
    {
        foreach (GameObject enemy in enemyPhase)
        {
            enemy.SetActive(false);
        }
    }

    /// <summary>
    /// Spawns all objects in the phase.
    /// </summary>
    public void SpawnObjects()
    {
        foreach (GameObject gameObject in objectPhase)
        {
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Checks if all enemies are dead.
    /// </summary>
    protected bool CheckEnemies()
    {
        int alive = enemyPhase.Count;

        foreach (GameObject enemy in enemyPhase)
        {
            if (PhaseManager.Instance.CheckAlive(enemy) == false)
            {
                alive--;
            }
        }

        if (alive == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
