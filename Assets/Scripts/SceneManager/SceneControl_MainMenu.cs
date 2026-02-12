using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneControl_MainMenu : SceneControlBase<SceneControl_MainMenu>
{
    public GameObject JoinGamePanel;
    public PlayerInputManager playerInputManager;

    //Character Selected Control
    public GameObject cutscene_sockThief;

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

    IEnumerator ShowJoinGamePanel()
    {
        yield return new WaitForSeconds(25f);
        JoinGamePanel.SetActive(true);
        GameManager.instance.isOnJoinGamePanel = true;
        playerInputManager.EnableJoining();
    }
}
