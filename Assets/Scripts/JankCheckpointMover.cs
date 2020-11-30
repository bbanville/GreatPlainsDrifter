using BrendonBanville.Tools;
using Opsive.UltimateCharacterController.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JankCheckpointMover : Singleton<JankCheckpointMover>
{
    public int currentPosID = 0;

    [Space(10)]
    public List<CheckpointPos> checkpointPositions;

    protected GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetSpawnPosition(int posID)
    {
        currentPosID = posID;
        gameObject.transform.position = checkpointPositions[posID].checkpointPosition;
        gameObject.transform.rotation = checkpointPositions[posID].checkpointRotation;
    }

    public void MovePlayerToSpawn()
    {
        //player.transform.position = gameObject.transform.position;
        //player.transform.rotation = gameObject.transform.rotation;

        player.transform.position = checkpointPositions[currentPosID].checkpointPosition;
        player.transform.rotation = checkpointPositions[currentPosID].checkpointRotation;
    }

    public void RespawnPlayer()
    {
        foreach (BattleSystem battle in checkpointPositions[currentPosID].Battles)
        {
            battle.ResetBattle();
        }

        MovePlayerToSpawn();
    }

    [System.Serializable]
    public class CheckpointPos
    {
        public string CheckpointName = "";
        public Vector3 checkpointPosition;
        public Quaternion checkpointRotation;
        public List<BattleSystem> Battles;
    }
}
