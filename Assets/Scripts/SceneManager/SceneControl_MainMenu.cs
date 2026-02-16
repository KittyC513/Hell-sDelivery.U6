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

    private void Start()
    {
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
        EnterPostOffice();
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

    public void EnterPostOffice()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SceneManager.LoadScene("PostOffice");   
        }
    }
}
