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

    public Camera cam_p1;
    public Camera cam_p2;

    public UIControl_NormalNPCs uiControl_P1;
    public UIControl_NormalNPCs uiControl_P2;

    public bool startConversation_p1 = false;
    public bool startConversation_p2 = false;

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
        player1.GetComponent<PlayerStateMachine>().FreezeStateMachine();
    }

    public void UnFreezePlayer1()
    {
        player1.GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
    }

    public void FreezePlayer2()
    {
        player2.GetComponent<PlayerStateMachine>().FreezeStateMachine();
    }

    public void UnFreezePlayer2()
    {
        player2.GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
    }
    public void FreezeBothPlayers()
    {

        if (player1 != null)
        {
            player1.GetComponent<PlayerStateMachine>().FreezeStateMachine();
            //p1cam.rect = changeRect; // Set Player 1's camera to full screen
        }

        if (player2 != null)
        {
            player2.GetComponent<PlayerStateMachine>().FreezeStateMachine();
            //p2cam.rect = new Rect(changeRect.width, 0, 1- changeRect.width, 1); // Set Player 2's camera to the right side of the screen
        }
        Debug.Log("Freeze both players");
    }

    public void UnFreezeBothPlayers()
    {
        if (player1 != null)
        {
            player1.GetComponent<PlayerStateMachine>().UnFreezeStateMachine();

            //p1cam.rect = new Rect(0, 0, 0.5f, 1); // Set Player 1's camera to full screen
        }

        if (player2 != null)
        {
            player2.GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
            //p2cam.rect = new Rect(0.5f, 0, 0.5f, 1);
        }

        Debug.Log("UnFreeze both players");
    }

    public void StartConversationWithNormalNpcs()
    {
        if (startConversation_p1)
        {
            player1.GetComponent<PlayerStateMachine>().FreezeStateMachine();
        }


        if (startConversation_p2)
        {
            player1.GetComponent<PlayerStateMachine>().FreezeStateMachine();
        }

    }

    public void EndConversationWithNormalNpcs()
    {
        if (player1 != null)
        {
            player1.GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
            cam_p1.rect = new Rect(0, 0, 0.5f, 1);

        }

        if (player2 != null)
        {
            player2.GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
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
