using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SceneControl_PostOffice : MonoBehaviour
{
    public Transform[] enterPoints;
    public DialogueSystemEvents dialogueSystemEvents_lalah;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

    }
    void Start()
    {
        //reset players position
        ResetPlayerPos();
        AddEventtoLalah();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPlayerPos()
    {
        GameManager.instance.player1.transform.position = enterPoints[0].position;
        GameManager.instance.player2.transform.position = enterPoints[1].position;
    }

    public void AddEventtoLalah()
    {
        dialogueSystemEvents_lalah.conversationEvents.onConversationStart.AddListener((actor) =>
        {
            GameManager.instance.FreezeBothPlayers();
        });

        dialogueSystemEvents_lalah.conversationEvents.onConversationEnd.AddListener((actor) =>
        {
            GameManager.instance.UnFreezeBothPlayers();
            EvenData.isAcceptedMission_lalah = true;
        });
    }
}
