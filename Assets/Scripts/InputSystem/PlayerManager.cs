using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Player spawn point manager
/// </summary>
public class PlayerManager : MonoBehaviour
{
    //private static PlayerManager instance;

    //public static PlayerManager Instance = instance;

    [HideInInspector]
    public List<PlayerInput> players = new List<PlayerInput>();

    [SerializeField]
    private List<Transform> startPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;
    [SerializeField]
    private List<LayerMask> colliderLayers;

    private PlayerInputManager playerInputManager;


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
        
        //Add colliders layer
        Transform collider = player.transform.Find("CollectHitbox");
        collider.gameObject.layer = (int)Mathf.Log(colliderLayers[players.Count - 1].value, 2);


        //set the layer
        //for future reference
        player.gameObject.layer = layerToAdd;

        //When 2nd player join, the camera will change the lock on camera viewport rect into half and half
        UpdatePlayerInfo();



        /******************************************************************************************************/
        //DataPersistenceManager.Instance.LoadGame();
    }


    //Modify lock on camera view regarding the numbers of player
    private void UpdatePlayerInfo()
    {
        GameManager gameManager = GameManager.instance;
        if (players.Count == 1) 
        {
            players[0].GetComponent<PlayerLockOn>().CameraManager.lockCam.rect = new Rect(0, 0, 0.5f, 1);

            if (gameManager != null)
            {
                GameManager.instance.player1 = players[0].gameObject; // Assign Player 1 to GameManager
                GameManager.instance.cam_p1 = players[0].GetComponent<PlayerInputDetection>().playerCam;
                GameManager.instance.uiControl_P1 = players[0].GetComponent<UIControl_NormalNPCs>();
                GameManager.instance.health_p1 = players[0].GetComponent<PlayerInputDetection>().health;
                GameManager.instance.stateMachine_p1 = players[0].GetComponent<PlayerStateMachine>();
                GameManager.instance.InputDetection_p1 = players[0].GetComponent<PlayerInputDetection>();
                GameManager.instance.itemControl_p1 = players[0].GetComponent<PlayerItemControl>();
            }
          

            //players[0].GetComponent<PlayerMoneyManager>().playerStats = player1Stats;
        }
        if (players.Count == 2)
        {

            players[1].GetComponent<PlayerLockOn>().CameraManager.lockCam.rect = new Rect(0.5f, 0, 0.5f, 1);


            if (gameManager != null)
            {
                GameManager.instance.player2 = players[1].gameObject; // Assign Player 2 to GameManager
                GameManager.instance.cam_p2 = players[1].GetComponent<PlayerInputDetection>().playerCam;
                GameManager.instance.uiControl_P2 = players[1].GetComponent<UIControl_NormalNPCs>();
                GameManager.instance.health_p2 = players[1].GetComponent<PlayerInputDetection>().health;
                GameManager.instance.stateMachine_p2 = players[1].GetComponent<PlayerStateMachine>();
                GameManager.instance.InputDetection_p2 = players[1].GetComponent<PlayerInputDetection>();
                GameManager.instance.itemControl_p2 = players[1].GetComponent<PlayerItemControl>();

            }
          

            //players[1].GetComponent<PlayerMoneyManager>().playerStats = player2Stats;

            #region Character Selection

            if (gameManager != null)
            {
                GameManager.instance.isOnCharacterSelection = true;
            }
            
            #endregion


        }
    }
}

