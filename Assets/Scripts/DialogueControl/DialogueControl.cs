using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;


public enum E_DialogueState
{
    None,
    WaitingForButtonPress_p1,
    WaitingForButtonPress_p2,
    TalkingToCamNPC,
}
public class DialogueControl : MonoBehaviour
{
    public Transform[] spawnPoints;
    private bool isResetPos = false;
    public Transform dialogueCam;

    [Header("Cam_NPC")]
    public DialogueSystemTrigger dialogueSystemTrigger_camNPC;
    public Usable dialogueUsable_camNPC;
    Collider[] c_players;
    public bool isTriggered_p1 = false;
    public bool isTriggered_p2 = false;

    public bool  isInRange_p1 = false;
    public bool  isInRange_p2 = false;

    public E_DialogueState currentDialogueState = E_DialogueState.None;

    private void Awake()
    {
        dialogueSystemTrigger_camNPC.enabled = false;
    }

    private void Start()
    {
        SwapBarkConversation("Old Coot/JunkyardEntrance");
    }

    private void Update()
    {
        OnButtonCheck();
        DialogueState();
        SetDialogueState();
    }

    public void OnConversation()
    {
        GameManager.instance.FreezeBothPlayers();
        GameManager.instance.DisableBothPlayersCam();
    }
    public void EndConversation()
    {
        GameManager.instance.UnFreezeBothPlayers();
        GameManager.instance.EnableBothPlayersCam();
        isResetPos = false;
    }

    public void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            print("resetting pos");
            Transform rotTransform = dialogueCam;
            GameManager.Instance.ResetPlayersPosition(spawnPoints[0], spawnPoints[1]);

            /***********rotation related to camera direction****************/
            GameManager.instance.RotatePlayersTo(dialogueCam);
            isResetPos = true;
        }
    }

    public void OnButtonCheck()
    {
        //Detect players in range of usable
        c_players = Physics.OverlapSphere(dialogueUsable_camNPC.transform.position, dialogueUsable_camNPC.maxUseDistance, 
            1 << LayerMask.NameToLayer("Player1") | 1 << LayerMask.NameToLayer("Player2"));

        if (c_players.Length > 0)
        {
            //check for each player if jump is pressed
            int p1Layer = LayerMask.NameToLayer("Player1");
            int p2Layer = LayerMask.NameToLayer("Player2");

            foreach (var col in c_players)
            {
                if (col.gameObject.layer == p1Layer)
                {
                    isInRange_p1 = true;
                    PlayerInputDetection inputDetection = col.GetComponent<PlayerInputDetection>();
                    if (inputDetection != null && inputDetection.jumpPressed)
                    {
                        isTriggered_p1 = true;
                    }              
                }

                if (col.gameObject.layer == p2Layer)
                {
                    isInRange_p2 = true;
                    PlayerInputDetection inputDetection = col.GetComponent<PlayerInputDetection>();
                    if (inputDetection != null && inputDetection.jumpPressed)
                    {
                        isTriggered_p2 = true;
                    }

                }
            }
        }
        else
        {
            isInRange_p1 = false;
            isInRange_p2 = false;
            isTriggered_p1 = false;
            isTriggered_p2 = false;
        }
    }

    public void SetDialogueState()
    {
        if(isTriggered_p1 && isTriggered_p2 && isInRange_p1 && isInRange_p2)
        {
            currentDialogueState = E_DialogueState.TalkingToCamNPC;
        }
        else if(isTriggered_p1 && isInRange_p1)
        {
            currentDialogueState = E_DialogueState.WaitingForButtonPress_p2;
        }
        else if(isTriggered_p2 && isInRange_p2)
        {
            currentDialogueState = E_DialogueState.WaitingForButtonPress_p1;
        }
        else
        {
            currentDialogueState = E_DialogueState.None;
        }
    }

    public void DialogueState()
    {
        switch(currentDialogueState)
        {
            case E_DialogueState.None:

                break;
            case E_DialogueState.WaitingForButtonPress_p1:
                print("Waiting for Button Press P1");
                break;
            case E_DialogueState.WaitingForButtonPress_p2:
                print("Waiting for Button Press P2");
                break;

            case E_DialogueState.TalkingToCamNPC:
                //handled by the dialogue system events
                print("dialogue trigger");
                dialogueSystemTrigger_camNPC.enabled = true;
                ResetPlayerPos();
                break;
        }
    }
    
    public void SwapBarkConversation(string conversation)
    {
        dialogueSystemTrigger_camNPC.barkConversation = conversation;
    }


}
