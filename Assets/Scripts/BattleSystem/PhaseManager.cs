using Opsive.UltimateCharacterController.Traits;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrendonBanville.Tools;

public class PhaseManager : Singleton<PhaseManager>
{
    public List<Phase> Phases;

    [SerializeField] protected bool CustomStartingPhase;
    [Condition("CustomStartingPhase", true)]
    [SerializeField] protected Phase StartingPhase;
    private Phase CurrentPhase;

    /// <summary>
    /// Awake is called once at the beginning of the scene
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (Phases == null)
        {
            enabled = false;
        }
    }

    /// <summary>
    /// Start is called on scene load
    /// </summary>
    protected void Start()
    {
        if (!CustomStartingPhase)
        {
            StartingPhase = Phases[0];
        }

        CurrentPhase = StartingPhase;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    protected void Update()
    {
        
    }

    public bool PrevPhaseComplete()
    {
        if (Phases[CurrentPhase.phaseID - 1].phaseComplete == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Spawns enemies and objects for the next uncompleted phase
    /// </summary>
    public void SpawnNextPhase()
    {
        Phase nextPhase = FindNextUncompletedPhase();

        CurrentPhase = nextPhase;

        nextPhase.SpawnEnemies();
    }

    /// <summary>
    /// Find the next uncompleted phase in the phase list.
    /// </summary>
    /// <returns>The next uncompleted phase in the phase list</returns>
    protected Phase FindNextUncompletedPhase()
    {
        foreach (Phase phase in Phases)
        {
            if (phase.phaseComplete == false)
            {
                return phase;
            }
        }

        Phase dummyPhase = new Phase();
        return dummyPhase;
    }
}
