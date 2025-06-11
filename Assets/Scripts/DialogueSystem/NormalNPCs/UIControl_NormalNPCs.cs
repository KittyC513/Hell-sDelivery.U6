using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands;
using PixelCrushers.DialogueSystem.Wrappers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PixelCrushers.DialogueSystem.StartConversationOnDialogueEvent;

public class UIControl_NormalNPCs : MonoBehaviour
{
    //public PixelCrushers.DialogueSystem.Usable usable;
    public PixelCrushers.DialogueSystem.DialogueSystemController dialogueController;
    public PixelCrushers.DialogueSystem.ProximitySelector proximitySelector;
    public Camera cam;
    public Rect camRect_P1;
    public Rect camRect_P2;

    public PlayerManager playerManager;
    public PlayerInputDetection playerInputDetection;

    public PixelCrushers.DialogueSystem.DialogueSystemTrigger dialogueSystemTrigger;
    public PlayerStateMachine playerStateMachine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        proximitySelector = this.GetComponent<PixelCrushers.DialogueSystem.ProximitySelector>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //if (proximitySelector.CurrentUsable != null)
        //{
        //    usable = proximitySelector.CurrentUsable;
        //}

    }


    /// <summary>
    /// Change conversant camera's viewport rect 
    /// </summary>
    public void UIControl()
    {
        if (proximitySelector.CurrentUsable.didStart)
        {
            if(this.gameObject.layer == LayerMask.NameToLayer("Player1"))
            {
                
                Debug.Log("Player1cam's rect changed");
                //PlayerManager.Instance.p2cam.rect = new Rect(0, camRect.y, 1 - camRect.width, 1);
            }
        }
    }

    /// <summary>
    /// Change both players' cam viewport rect when player intects with the Normal Npcs
    /// </summary>
    public void SwitchToConversantCamRect()
    {
        if (proximitySelector.usablesInRange != null && playerInputDetection.crouchPressed)
        {
            if (this.gameObject.layer == LayerMask.NameToLayer("Player1"))
            {
                dialogueSystemTrigger = proximitySelector.usablesInRange[0].GetComponent<PixelCrushers.DialogueSystem.DialogueSystemTrigger>();
                dialogueSystemTrigger.enabled = true; // Enable the DialogueSystemTrigger to start the conversation

                Debug.Log("Player1cam's rect changed");
                playerManager.p1cam.rect = camRect_P1;
                playerManager.p2cam.rect = new Rect(camRect_P1.width, 0, 1 - camRect_P1.width, 1);
                playerManager.players[0].GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
                playerManager.startConversation_p1 = true;
            }

            if (this.gameObject.layer == LayerMask.NameToLayer("Player2"))
            {
                dialogueSystemTrigger = proximitySelector.usablesInRange[0].GetComponent<PixelCrushers.DialogueSystem.DialogueSystemTrigger>();
                dialogueSystemTrigger.enabled = true; // Enable the DialogueSystemTrigger to start the conversation
                Debug.Log("Player2cam's rect changed");

                playerManager.p2cam.rect = camRect_P2;
                playerManager.p1cam.rect = new Rect(0, 0, 1 - camRect_P2.width, 1);
                playerManager.players[1].GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
                playerManager.startConversation_p2 = true;
            }
            //print("ButtonDown" + proximitySelector.IsUseButtonDown());
            //if (this.gameObject.layer == LayerMask.NameToLayer("Player1"))
            //{
            //    playerManager.p1cam.rect = camRect;
            //    Debug.Log("Player1cam's rect changed");
            //    playerManager.p2cam.rect = new Rect(camRect.width, 0, 1 - camRect.width, 1);
            //    playerManager.StartConversation();
            //}

            //if (this.gameObject.layer == LayerMask.NameToLayer("Player2"))
            //{
            //    playerManager.p2cam.rect = camRect;
            //    Debug.Log("Player2cam's rect changed");
            //    playerManager.p1cam.rect = new Rect(camRect.width, 0, 1 - camRect.width, 1);
            //    playerManager.StartConversation();
            //}
        }
    }
}
