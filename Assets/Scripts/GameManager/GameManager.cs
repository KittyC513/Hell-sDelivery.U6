using UnityEngine;
using UnityEngine.InputSystem.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Character Selection")]
    public bool onCharacterSelection = false;
    public CharacterSelectionPanel characterSelectionPanel;
    public MultiplayerEventSystem multiplayerEventSystem_p1;
    public MultiplayerEventSystem multiplayerEventSystem_p2;

    public PlayerManager playerManager;

    [Header("PlayerData")]
    public GameObject player1;
    public GameObject player2;

    public Camera cam_p1;
    public Camera cam_p2;

    public UIControl_NormalNPCs uiControl_p1;
    public UIControl_NormalNPCs uiControl_p2;

    public bool startConversation_p1 = false;
    public bool startConversation_p2 = false;

    public PlayerStats player1Stats;
    public PlayerStats player2Stats;

    [Header("Scene Control")]
    public bool sceneChanged = false;


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
        if (onCharacterSelection)
        {
            print("Character Selection is on");
            CharacterSelection();
        }


    }

    #region Character Selection Panel

    #endregion
    private void CharacterSelection()
    {
        characterSelectionPanel.gameObject.SetActive(true);
        FreezeBothPlayers();

        //if (playerManager.players[0].GetComponent<PlayerInputDetection>().horizontalInputValue.x > 0.2f && !characterSelectionPanel.p1Selected_rightScreen)
        //{
        //    characterSelectionPanel.p1Selected_rightScreen = true;
        //}
        //if (playerManager.players[2].GetComponent<PlayerInputDetection>().horizontalInputValue.x < -0.2f && !characterSelectionPanel.p2Selected_leftScreen)
        //{
        //    characterSelectionPanel.p2Selected_leftScreen = true;
        //}
        //if (playerManager.players[1].GetComponent<PlayerInputDetection>().horizontalInputValue.x > 0.2f && !characterSelectionPanel.p1Selected_rightScreen)
        //{
        //    characterSelectionPanel.p1Selected_rightScreen = true;
        //}
        //if (playerManager.players[2].GetComponent<PlayerInputDetection>().horizontalInputValue.x < -0.2f && !characterSelectionPanel.p2Selected_leftScreen)
        //{
        //    characterSelectionPanel.p2Selected_leftScreen = true;
        //}

    }

    #region Conversation Function
    /// <summary>
    /// Progress NPCs
    /// </summary>
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
        Debug.Log("StartConversation");
    }

    public void UnfreezeBothPlayers()
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

        Debug.Log("EndConversation");
    }

    public void StartConversationWithNormalNpcs()
    {
        if (startConversation_p1)
        {
            player1.GetComponent<PlayerStateMachine>().FreezeStateMachine();
        }


        if (startConversation_p2)
        {
            player2.GetComponent<PlayerStateMachine>().FreezeStateMachine();
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
            uiControl_p1.dialogueSystemTrigger.enabled = false; // Disable the DialogueSystemTrigger to stop the conversation
            uiControl_p1.dialogueSystemTrigger = null;
            startConversation_p1 = false;
        }


        if (startConversation_p2)
        {
            uiControl_p2.dialogueSystemTrigger.enabled = false; // Disable the DialogueSystemTrigger to stop the conversation
            uiControl_p2.dialogueSystemTrigger = null;
            startConversation_p2 = false;
        }


    }
    #endregion

}
