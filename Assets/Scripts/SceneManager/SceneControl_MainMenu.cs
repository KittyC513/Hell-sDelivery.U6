using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneControl_MainMenu : SceneControlBase<SceneControl_MainMenu>
{
    public GameObject JoinGamePanel;
    public PlayerInputManager playerInputManager;

    public Transform[] spawnpoints;

    //Character Selected Control
    public GameObject cutscene_sockThief;
    public GameObject cinematicCanvas;
    public GameObject cutscene_tutorialEnd;

    public Camera cam_cutscene_sockThief;

    private void Start()
    {
        cutscene_tutorialEnd.SetActive(false);

        playerInputManager.DisableJoining();
        JoinGamePanel.SetActive(false);
        EventData.curSceneName = "StartScene";
        StartCoroutine(ShowJoinGamePanel());

        //cutscene_characterSelected.SetActive(false); 
        cutscene_sockThief.SetActive(false);
        //cam_cutscene_characterSelected.gameObject.SetActive(false);
    }

    private void Update()
    {
        OnDevTesting();
    }

    IEnumerator ShowJoinGamePanel()
    {
        yield return new WaitForSeconds(25f);
        JoinGamePanel.SetActive(true);
        GameManager.instance.isOnJoinGamePanel = true;
        playerInputManager.EnableJoining();
    }

    public void OnGameStart()
    {
        GameManager.instance.ResetPlayer1Position(spawnpoints[0]);
        GameManager.instance.ResetPlayer2Position(spawnpoints[1]);
        GameManager.instance.UnFreezeBothPlayers();
        EventData.gameStart = true;
    }

    public void OnDevTesting()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            JoinGamePanel.SetActive(true);
            playerInputManager.EnableJoining();
            GameManager.instance.isOnJoinGamePanel = true;
        }

        EnterPostOffice();
        EnterGreyBox();
    }
    public void EnterPostOffice()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SceneManager.LoadScene("PostOffice");
        }
    }

    public void EnterGreyBox()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SceneManager.LoadScene("Level_greyBox");
        }
    }

    
}
