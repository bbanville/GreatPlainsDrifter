
using BrendonBanville.Tools;
using Opsive.UltimateCharacterController.Demo.BehaviorDesigner;
//using Opsive.UltimateCharacterController.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opsive.UltimateCharacterController.Demo
{
    public class BattleSystem : MonoBehaviour
    {
        [SerializeField] protected DemoZoneTrigger startBattleTrigger;

        //[ReadOnly] public EventHandler OnBattleStarted;
        public event EventHandler OnBattleStarted;
        //[ReadOnly] public EventHandler OnBattleEnded;
        public event EventHandler OnBattleEnded;

        private enum State
        {
            WaitingToSpawn,
            Active,
            BattleOver,
        }

        /// <summary>
        /// Represents a single Enemy Spawn Wave
        /// </summary>
        [System.Serializable]
        public class Wave
        {
            public string waveTitle;

            [Header("Enemies")]
            public Transform enemySpawnContainer;
            public DemoAgent[] enemyAgentArray;

            [Header("Spawn Condition")]
            [Tooltip("Delay from start of battle before spawning this wave")]
            public float time;

            [Tooltip("Has the enemies in this wave already been spawned?")]
            [ReadOnly] public bool alreadySpawned;
        }

        [SerializeField] private Wave[] waveArray;

        //[SerializeField] private DoorAnims doorAnims;

        private State state;
        private List<DemoAgent> enemyAgentList;

        private void Awake()
        {
            state = State.WaitingToSpawn;
            enemyAgentList = new List<DemoAgent>();
        }

        private void Start()
        {
            Debug.Log("Battle Registered");
            startBattleTrigger.OnPlayerTriggerEnter += StartBattleTrigger_OnPlayerTriggerEnter;
            //EventHandler.RegisterEvent("StartBattleTrigger_OnPlayerTriggerEnter", StartBattleTrigger_OnPlayerTriggerEnter);
        }

        private void StartBattleTrigger_OnPlayerTriggerEnter(object sender, System.EventArgs e)
        {
            StartBattle();
            Debug.Log("Battle Triggered");
            startBattleTrigger.OnPlayerTriggerEnter -= StartBattleTrigger_OnPlayerTriggerEnter;
            // EventHandler.UnregisterEvent("StartBattleTrigger_OnPlayerTriggerEnter", StartBattleTrigger_OnPlayerTriggerEnter);
        }

        private void Update()
        {
            switch (state)
            {
                case State.Active:
                    Debug.Log("Battle Active");

                    foreach (Wave wave in waveArray)
                    {
                        if (wave.alreadySpawned) continue; // Wave already spawned
                        wave.time -= Time.deltaTime;
                        if (wave.time <= 0f)
                        {
                            wave.alreadySpawned = true;
                            SpawnWave(wave);
                        }
                    }
                    break;
            }
        }

        private void SpawnWave(Wave wave)
        {
            List<DemoAgent> waveSpawnEnemyList = new List<DemoAgent>();
            if (wave.enemySpawnContainer != null)
            {
                foreach (Transform transform in wave.enemySpawnContainer)
                {
                    DemoAgent enemySpawn = transform.GetComponent<DemoAgent>();
                    if (enemySpawn != null)
                    {
                        waveSpawnEnemyList.Add(enemySpawn);
                    }
                }
            }

            if (wave.enemyAgentArray != null)
            {
                waveSpawnEnemyList.AddRange(wave.enemyAgentArray);
            }

            foreach (DemoAgent enemyAgent in waveSpawnEnemyList)
            {
                enemyAgent.Spawn();
                //enemyAgent.OnDead += DemoAgent_OnDead;
                enemyAgentList.Add(enemyAgent);
            }
        }

        private void StartBattle()
        {
            state = State.Active;

            //if (doorAnims != null)
            //{
            //    doorAnims.SetColor(DoorAnims.ColorName.Red);
            //    doorAnims.CloseDoor();
            //}

            //EventHandler.ExecuteEvent("OnBattleStarted");
            OnBattleStarted?.Invoke(this, EventArgs.Empty);
        }

        private void EndBattle()
        {
            //if (doorAnims != null)
            //{
            //    doorAnims.SetColor(DoorAnims.ColorName.Green);
            //    FunctionTimer.Create(doorAnims.OpenDoor, 1.5f);
            //}

            //EventHandler.ExecuteEvent("OnBattleEnded");
            OnBattleEnded?.Invoke(this, EventArgs.Empty);
        }

        private void EnemySpawn_OnDead(object sender, System.EventArgs e)
        {
            TestBattleOver();
        }

        private void TestBattleOver()
        {
            foreach (DemoAgent enemySpawn in enemyAgentList)
            {
                if (enemySpawn.IsAlive())
                {
                    // Still alive
                    return;
                }
            }

            // All dead!
            EndBattle();
        }
    }
}
