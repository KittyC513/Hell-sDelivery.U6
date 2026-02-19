using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    //Add all checkpoints as a child of this object
    private List<Transform> checkpointPositions;
    private RespawnAndDeath[] playerRespawns;


    public int checkpointIndex = 0;

    private void Start()
    {
        checkpointPositions = new List<Transform>();
        playerRespawns = new RespawnAndDeath[2];

        //temporary slow way to grab the player respawns
        playerRespawns[0] = GameManager.instance.PlayerController_p1.GetComponentInChildren<RespawnAndDeath>();
        playerRespawns[1] = GameManager.instance.PlayerController_p2.GetComponentInChildren<RespawnAndDeath>();

        for (int i = 0; i < transform.childCount; i++)
        {
            //add checkpoints to a list
            checkpointPositions.Add(transform.GetChild(i));
        }
    }

    public void RespawnPlayers()
    {
        
        for (int i = 0; i < playerRespawns.Length; i++)
        {
            if (checkpointIndex < checkpointPositions.Count)
            {
                playerRespawns[i].respawnPosition = checkpointPositions[checkpointIndex].position;
            }
            else
            {
                playerRespawns[i].respawnPosition = checkpointPositions[checkpointPositions.Count - 1].position;
            }

            if (checkpointIndex < 0)
            {
                playerRespawns[i].respawnPosition = checkpointPositions[0].position;
            }

            playerRespawns[i].Respawn();
        }
    }
}
