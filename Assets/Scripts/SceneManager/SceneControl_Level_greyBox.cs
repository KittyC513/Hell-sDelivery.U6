using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SceneControl_Level_greyBox : SceneControlBase<SceneControl_Level_greyBox>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform[] spawnPoints;
    public DialogueSystemTrigger dialogueSystem_devil;
    public DialogueSystemController dialogueSystemController;

    void Start()
    {
        EventData.curSceneName = "Level_greyBox";
        //dialogueSystemController.SetContinueMode(false);
        //dialogueSystemController.SetOriginalContinueMode();
    }

    // Update is called once per frame
    void Update()
    {
        ResetPlayerPos();
    }

    void ResetPlayerPos()
    {
        if(!isResetPos)
        {
            GameManager.Instance.ResetPlayersPosition(spawnPoints[0], spawnPoints[1]);
            isResetPos = true;
        }
    }

    public void EnableDevilDialogue()
    {
        dialogueSystem_devil.enabled = true;
    }

    public void DisableDevilDialogue()
    {
        dialogueSystem_devil.enabled = false;
    }

    public void SwapBarkConversation(string conversation)
    {
        dialogueSystem_devil.barkConversation = conversation;
       
    }
}
