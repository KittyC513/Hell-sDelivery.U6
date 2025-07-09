using UnityEngine;
using UnityEngine.InputSystem.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Character Selection")]
    public GameObject characterSelectPanel;
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

    public Camera cam_p1;
    public Camera cam_p2;

    public UIControl_NormalNPCs uiControl_P1;
    public UIControl_NormalNPCs uiControl_P2;

    public bool startConversation_p1 = false;
    public bool startConversation_p2 = false;

    public Health health_p1;
    public Health health_p2;


    private void Awake()
    {  
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isOnCharacterSelection)
        {
            characterSelectPanel.SetActive(true);
        }

    }

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
    #endregion

}
