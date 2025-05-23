using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Player spawn point manager
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [HideInInspector]
    public List<PlayerInput> players = new List<PlayerInput>();

    [SerializeField]
    private List<Transform> startPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;
    public float lockOnCam_height = 3.39f;
    public float lockOnCam_distance = 14.5f;

    private void Awake()
    {
        playerInputManager = FindFirstObjectByType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        //add player when join button has been pressed
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        //delete player count when it's disabled
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        //Set player's(whole player prefab) start position to the spawn point
        Transform playerParent = player.transform.parent;
        playerParent.position = startPoints[players.Count - 1].position;

        //convert layer mask to an int
        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        //set the layer
        //for future reference
        player.gameObject.layer = layerToAdd;

        //When 2nd player join, the camera will change the lock on camera viewport rect into half and half
        AdaptLockOnCamermView();
    }


    //Modify lock on camera view regarding the numbers of player
    private void AdaptLockOnCamermView()
    {
        if(players.Count == 2)
        {
            players[0].GetComponent<PlayerLockOn>().CameraManager.lockCam.rect = new Rect(0, 0, 0.5f, 1);
            players[0].GetComponent<PlayerLockOn>().CameraManager.cameraMovement_Lock.distance = lockOnCam_distance;
            players[0].GetComponent<PlayerLockOn>().CameraManager.cameraMovement_Lock.height = lockOnCam_height;
            players[1].GetComponent<PlayerLockOn>().CameraManager.lockCam.rect = new Rect(0.5f, 0, 0.5f, 1);
            players[1].GetComponent<PlayerLockOn>().CameraManager.cameraMovement_Lock.distance = lockOnCam_distance;
            players[1].GetComponent<PlayerLockOn>().CameraManager.cameraMovement_Lock.height = lockOnCam_height;

        }
    }

    public void StartConversation()
    {
       
        if(players[0] != null)
        {
            players[0].GetComponent<PlayerStateMachine>().FreezeStateMachine();

        }

        if (players[1] != null)
        {
            players[1].GetComponent<PlayerStateMachine>().FreezeStateMachine();
        }
        Debug.Log("StartConversation");
    }

    public void EndConversation()
    {
        if (players[0] != null)
        {
            players[0].GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
        }

        if (players[1] != null)
        {
            players[1].GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
        }
        Debug.Log("EndConversation");
    }
}

