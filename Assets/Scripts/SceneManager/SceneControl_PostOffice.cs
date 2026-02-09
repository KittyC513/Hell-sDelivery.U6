using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SceneControl_PostOffice : SceneControlBase<SceneControl_PostOffice>
{
    public Transform[] spawnpoints;
    public DialogueSystemEvents dialogueSystemEvents_lalah;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

    }
    void Start()
    {
        EventData.curSceneName = "PostOffice";
        //reset players position

    }

    // Update is called once per frame
    void Update()
    {
        ResetPlayerPos();
    }

    public void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            GameManager.instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
            isResetPos = true;
        }
    }

    #region Conversation Even
    public void AddEventtoLalah()
    {
        //dialogueSystemEvents_lalah.conversationEvents.onConversationStart.AddListener((actor) =>
        //{
        //    GameManager.instance.StartConversationWithNormalNpcs();
        //});

        //dialogueSystemEvents_lalah.conversationEvents.onConversationEnd.AddListener((actor) =>
        //{
        //    GameManager.instance.EndConversationWithNormalNpcs();
        //    EventData.isAcceptedMission_lalah = true;
        //    print("EventData.isAcceptedMission_lalah:" + EventData.isAcceptedMission_lalah);
        //});
    }
    #endregion



}
