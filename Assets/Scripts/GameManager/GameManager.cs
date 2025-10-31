using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager
{
    public static GameManager instance = new GameManager();
    public static GameManager Instance => instance;

    [Header("Character Selection")]
    public bool isOnCharacterSelection = false;

    [Header("Player Data")]
    public GameObject player1;
    public GameObject player2;

    public PlayerInputDetection InputDetection_p1;
    public PlayerInputDetection InputDetection_p2;

    public PlayerItemControl itemControl_p1;
    public PlayerItemControl itemControl_p2;

    public PlayerStateMachine stateMachine_p1;
    public PlayerStateMachine stateMachine_p2;

    public Bag bag_p1;
    public Bag bag_p2;

    public Camera cam_p1;
    public Camera cam_p2;

    public UIControl_NormalNPCs uiControl_P1;
    public UIControl_NormalNPCs uiControl_P2;

    public bool startConversation_p1 = false;
    public bool startConversation_p2 = false;

    public Health health_p1;
    public Health health_p2;

    [Header("PointerControl")]
    public UIPointerControl uiPointerControl_p1;
    public UIPointerControl uiPointerControl_p2;
    public bool pointerIsReady = false;


    [Header("Scene Info")]
    public bool isSceneChanged = false;

    [Header("Crane")]
    public GameObject craneObject;




    #region Conversation Control
    /// <summary>
    /// Progress NPCs
    /// </summary>
    /// 
    public void FreezePlayer1()
    {
        stateMachine_p1.FreezeStateMachine();
    }

    public void UnFreezePlayer1()
    {
        stateMachine_p1.UnFreezeStateMachine();
    }

    public void FreezePlayer2()
    {
        stateMachine_p2.FreezeStateMachine();
    }

    public void UnFreezePlayer2()
    {
        stateMachine_p2.UnFreezeStateMachine();
    }
    public void FreezeBothPlayers()
    {

        if (player1 != null)
        {
            stateMachine_p1.FreezeStateMachine();
            //p1cam.rect = changeRect; // Set Player 1's camera to full screen
        }

        if (player2 != null)
        {
            stateMachine_p2.FreezeStateMachine();
            //p2cam.rect = new Rect(changeRect.width, 0, 1- changeRect.width, 1); // Set Player 2's camera to the right side of the screen
        }
        Debug.Log("Freeze both players");
    }

    public void UnFreezeBothPlayers()
    {
        if (player1 != null)
        {
            stateMachine_p1.UnFreezeStateMachine();

            //p1cam.rect = new Rect(0, 0, 0.5f, 1); // Set Player 1's camera to full screen
        }

        if (player2 != null)
        {
            stateMachine_p2.UnFreezeStateMachine();
            //p2cam.rect = new Rect(0.5f, 0, 0.5f, 1);
        }

        Debug.Log("UnFreeze both players");
    }

    public void StartConversationWithNormalNpcs()
    {
        if (startConversation_p1)
        {
            stateMachine_p1.FreezeStateMachine();
        }


        if (startConversation_p2)
        {
            stateMachine_p2.FreezeStateMachine();
        }

    }

    public void EndConversationWithNormalNpcs()
    {
        if (player1 != null)
        {
            stateMachine_p1.UnFreezeStateMachine();
            cam_p1.rect = new Rect(0, 0, 0.5f, 1);

        }

        if (player2 != null)
        {
            stateMachine_p2.UnFreezeStateMachine();
            cam_p2.rect = new Rect(0.5f, 0, 0.5f, 1);
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

    public void DisableBothPlayersCam()
    {
        cam_p1.enabled = false;
        cam_p2.enabled = false;
    }

    public void EnableBothPlayersCam()
    {
        cam_p1.enabled = true;
        cam_p2.enabled = true;
    }

    #endregion

    #region Pointer Control
    public void AssignPointerToPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uiPointerControl_p1.targetPlayer = player2.transform;
            Sprite sprPointer = Resources.Load<Sprite>("Art/UI/Indicator/Indicator_P2");
            uiPointerControl_p1.playerPointer.GetComponent<Image>().sprite = sprPointer;


            uiPointerControl_p2.targetPlayer = player1.transform;       
            sprPointer = Resources.Load<Sprite>("Art/UI/Indicator/Indicator_P1");
            uiPointerControl_p2.playerPointer.GetComponent <Image>().sprite = sprPointer;

            pointerIsReady = true;

            Debug.Log("pointerIsReady" + pointerIsReady);
        }
    }

    #endregion

    #region Player 

    public void ResetPlayersPosition(Transform p1SpawnPoint, Transform p2SpawnPoint)
    {
        if (player1 != null)
        {
            player1.transform.position = p1SpawnPoint.position;
        }
        if (player2 != null)
        {
            player2.transform.position = p2SpawnPoint.position;
        }
    }
    #endregion

}
