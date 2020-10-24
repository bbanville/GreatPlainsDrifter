using BrendonBanville.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Phase : MonoBehaviour
{
    [Header("System Tracking")]
    [SerializeField] protected int phaseID = 0;

    [Header("Object Lists")]
    [SerializeField] protected List<GameObject> enemyPhase;
    [SerializeField] protected List<GameObject> objectPhase;

    [ReadOnly] public bool PhaseComplete = false;
    [ReadOnly] private bool PhaseSpawned = false;

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
        if (!PhaseComplete)
        {
            if (!PhaseSpawned)
            {
                SpawnPhase();
                PhaseSpawned = true;
            }

            if (CheckEnemies() == true)
            {
                PhaseComplete = true;
                DespawnEnemies();
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
    public void SpawnPhase()
    {
        foreach (GameObject enemy in enemyPhase)
        {
            enemy.SetActive(true);
        }

        foreach (GameObject gameObject in objectPhase)
        {
            gameObject.SetActive(true);
        }
    }

    protected void DespawnEnemies()
    {
        foreach (GameObject enemy in enemyPhase)
        {
            enemy.SetActive(false);
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
