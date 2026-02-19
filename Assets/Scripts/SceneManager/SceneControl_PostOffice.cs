using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneControl_PostOffice : SceneControlBase<SceneControl_PostOffice>
{
    public Transform[] spawnpoints;
    public DialogueSystemEvents dialogueSystemEvents_lalah;
    public float cutsceneDuration = 18f;
    public Camera playCam;

    public GameObject cutscene_enterOffice;
    public GameObject canvas_fadeOut;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

    }
    void Start()
    {
        playCam.enabled = false;

        GameManager.instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
        GameManager.instance.FreezeBothPlayers();
        EventData.curSceneName = "PostOffice";
        //reset players position
        if(!isResetPos)
            StartCoroutine(OnCutSceneEnd());


    }

    // Update is called once per frame
    void Update()
    {

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

    IEnumerator OnCutSceneEnd()
    {
        yield return new WaitForSeconds(cutsceneDuration);
        cutscene_enterOffice.SetActive(false);
        isResetPos = true;
    }

    public void SwitchCamera()
    {
        playCam.enabled = true;
        GameManager.instance.ResetPlayersPosition(spawnpoints[2], spawnpoints[3]);
        GameManager.instance.RotatePlayersTo(spawnpoints[2]);
        GameManager.instance.UnFreezeBothPlayers();
        canvas_fadeOut.SetActive(false);

    }

    #region Scene Loading
    public void LoadNextScene()
    {
        StartCoroutine(StartLoadingScene());
    }

    IEnumerator StartLoadingScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Level_greyBox");
    }

    public void FreezePlayers()
    {
        GameManager.instance.FreezeBothPlayers();
    }

    public void UnFreezePlayers()
    {
        GameManager.instance.UnFreezeBothPlayers();
    }

    IEnumerator StartTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        canvas_fadeOut.SetActive(false);
    }

    #endregion


}
