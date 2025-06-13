using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Player spawn point manager
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;

    public static PlayerManager Instance = instance;

    [HideInInspector]
    public List<PlayerInput> players = new List<PlayerInput>();

    [SerializeField]
    private List<Transform> startPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;
    //public float lockOnCam_height = 3.39f;
    //public float lockOnCam_distance = 14.5f;

    public Camera p1cam;
    public Camera p2cam;

    public UIControl_NormalNPCs uiControl_P1;
    public UIControl_NormalNPCs uiControl_P2;

    public Rect changeRect_P1 = new Rect(0,0,0.7f,1);
    public Rect changeRect_P2 = new Rect(0.3f, 0, 0.7f, 1);

    public bool startConversation_p1 = false;
    public bool startConversation_p2 = false;

    [SerializeField] private PlayerStats player1Stats;
    [SerializeField] private PlayerStats player2Stats;

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

        //if(uiControl_P1 != null && uiControl_P2 != null)
        //{
        //    if (uiControl_P1.cam != null && uiControl_P2.cam != null)
        //        Camera.main.enabled = false;
        //}
    }


    //Modify lock on camera view regarding the numbers of player
    private void AdaptLockOnCamermView()
    {
        if(players.Count == 2)
        {
            players[0].GetComponent<PlayerLockOn>().CameraManager.lockCam.rect = new Rect(0, 0, 0.5f, 1);
            //players[0].GetComponent<PlayerLockOn>().CameraManager.cameraMovement_Lock.distance = lockOnCam_distance;
            //players[0].GetComponent<PlayerLockOn>().CameraManager.cameraMovement_Lock.height = lockOnCam_height;
            uiControl_P1 = players[0].GetComponent<UIControl_NormalNPCs>();
            players[0].GetComponent<UIControl_NormalNPCs>().playerManager = this;
            p1cam = players[0].GetComponent<PlayerInputDetection>().playerCam;
            players[1].GetComponent<PlayerLockOn>().CameraManager.lockCam.rect = new Rect(0.5f, 0, 0.5f, 1);
            //players[1].GetComponent<PlayerLockOn>().CameraManager.cameraMovement_Lock.distance = lockOnCam_distance;
            //players[1].GetComponent<PlayerLockOn>().CameraManager.cameraMovement_Lock.height = lockOnCam_height;
            uiControl_P2 = players[1].GetComponent<UIControl_NormalNPCs>();
            players[1].GetComponent<UIControl_NormalNPCs>().playerManager = this;
            p2cam = players[1].GetComponent<PlayerInputDetection>().playerCam;

            players[0].GetComponent<PlayerMoneyManager>().playerStats = player1Stats;
            players[1].GetComponent<PlayerMoneyManager>().playerStats = player2Stats;
        }
    }

    /// <summary>
    /// Progress NPCs
    /// </summary>
    public void StartConversation()
    {
       
        if(players[0] != null)
        {
            players[0].GetComponent<PlayerStateMachine>().FreezeStateMachine();
            //p1cam.rect = changeRect; // Set Player 1's camera to full screen
        }

        if (players[1] != null)
        {
            players[1].GetComponent<PlayerStateMachine>().FreezeStateMachine();
            //p2cam.rect = new Rect(changeRect.width, 0, 1- changeRect.width, 1); // Set Player 2's camera to the right side of the screen
        }
        Debug.Log("StartConversation");
    }

    public void EndConversation()
    {
        if (players[0] != null)
        {
            players[0].GetComponent<PlayerStateMachine>().UnFreezeStateMachine();

            //p1cam.rect = new Rect(0, 0, 0.5f, 1); // Set Player 1's camera to full screen
        }

        if (players[1] != null)
        {
            players[1].GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
            //p2cam.rect = new Rect(0.5f, 0, 0.5f, 1);
        }

        Debug.Log("EndConversation");
    }

    public void StartConversationWithNormalNpcs()
    {
        if (startConversation_p1)
        {
            players[0].GetComponent<PlayerStateMachine>().FreezeStateMachine();
        }


        if (startConversation_p2)
        {
            players[1].GetComponent<PlayerStateMachine>().FreezeStateMachine();
        }

    }

    public void EndConversationWithNormalNpcs()
    {
        if (players[0] != null)
        {
            players[0].GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
            p1cam.rect = new Rect(0, 0, 0.5f, 1);

        }

        if (players[1] != null)
        {
            players[1].GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
            p2cam.rect = new Rect(0.5f, 0, 0.5f, 1);
        }

        if (startConversation_p1)
        {
            uiControl_P1.dialogueSystemTrigger.enabled = false; // Disable the DialogueSystemTrigger to stop the conversation
            uiControl_P1.dialogueSystemTrigger = null;
            startConversation_p1 = false;
        }

        
        if (startConversation_p2)
        {
            uiControl_P2.dialogueSystemTrigger.enabled = false; // Disable the DialogueSystemTrigger to stop the conversation
            uiControl_P2.dialogueSystemTrigger = null;
            startConversation_p2 = false;
        }


    }
}

