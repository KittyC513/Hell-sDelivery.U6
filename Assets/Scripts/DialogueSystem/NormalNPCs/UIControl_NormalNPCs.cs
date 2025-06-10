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
    public Rect camRect;
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


    public void UIControl()
    {
        if (proximitySelector.CurrentUsable.didStart)
        {
            if(this.gameObject.layer == LayerMask.NameToLayer("Player1"))
            {
                cam.rect = camRect;
                Debug.Log("Player1cam's rect changed");
                //PlayerManager.Instance.p2cam.rect = new Rect(0, camRect.y, 1 - camRect.width, 1);
            }
        }
    }
}
