using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneControl_MainMenu : SceneControlBase<SceneControl_MainMenu>
{
    public GameObject characterSelectPanel;
    public GameObject JoinGamePanel;
    public PlayerInputManager playerInputManager;

    //Character Selected Control
    public GameObject cutscene_characterSelected;

    private void Start()
    {
        playerInputManager.DisableJoining();
        JoinGamePanel.SetActive(false);
        EventData.curSceneName = "StartScene";
        StartCoroutine(ShowJoinGamePanel());

        cutscene_characterSelected.SetActive(false);
        //cam_cutscene_characterSelected.gameObject.SetActive(false);
    }

    IEnumerator ShowJoinGamePanel()
    {
        yield return new WaitForSeconds(16.7f);
        JoinGamePanel.SetActive(true);
        GameManager.instance.isOnJoinGamePanel = true;
        playerInputManager.EnableJoining();
    }
}
