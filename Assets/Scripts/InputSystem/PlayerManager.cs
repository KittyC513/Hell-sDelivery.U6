using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem.UI;

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

    public Rect changeRect_P1 = new Rect(0,0,0.7f,1);
    public Rect changeRect_P2 = new Rect(0.3f, 0, 0.7f, 1);


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
        print("Player " + players.Count + " has joined the game");


        //When 2nd player join, the camera will change the lock on camera viewport rect into half and half       
        InitializePlayerInfo();



        /******************************************************************************************************/
        //DataPersistenceManager.Instance.LoadGame();
    }

    //Modify lock on camera view regarding the numbers of player
    private void InitializePlayerInfo()
    {
        if(players.Count == 1)
        {
            //Gain p1 data
            //players[0].GetComponent<PlayerLockOn>().CameraManager.lockCam.rect = new Rect(0, 0, 0.5f, 1);
            GameManager.instance.player1 = players[0].gameObject;
            GameManager.instance.cam_p1 = players[0].GetComponent<PlayerInputDetection>().playerCam;
            GameManager.instance.uiControl_p1 = players[0].GetComponent<UIControl_NormalNPCs>();

            //players[0].GetComponent<PlayerMoneyManager>().playerStats = GameManager.instance.player1Stats;


        }

        if (players.Count == 2)
        {         
            //Gain p2 data
            //players[1].GetComponent<PlayerLockOn>().CameraManager.lockCam.rect = new Rect(0.5f, 0, 0.5f, 1);
            GameManager.instance.player2 = players[1].gameObject;
            GameManager.instance.cam_p2 = players[1].GetComponent<PlayerInputDetection>().playerCam;
            GameManager.instance.uiControl_p2 = players[1].GetComponent<UIControl_NormalNPCs>();
            
            //players[1].GetComponent<PlayerMoneyManager>().playerStats = GameManager.instance.player2Stats;

            print("Player2 is here");

            //When 2nd player has joined, the game will enter character selection panel
            //GameManager.instance.onCharacterSelection = true;
        }

    }

}

