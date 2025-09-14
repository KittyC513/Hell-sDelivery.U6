using System.Collections;
using UnityEngine;

public class SceneControl_MainMenu : SceneControlBase<SceneControl_MainMenu>
{
    public GameObject characterSelectPanel;
    public GameObject JoinGamePanel;
    public GameObject cutscene_characterSelected;
    public Camera cam_cutscene_characterSelected;
    private void Start()
    {
        JoinGamePanel.SetActive(false);
        EventData.curSceneName = "StartScene";
        StartCoroutine(ShowJoinGamePanel());

        cutscene_characterSelected.SetActive(false);
        //cam_cutscene_characterSelected.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isOnCharacterSelection)
        {
            //characterSelectPanel.SetActive(true);
        }
    }

    IEnumerator ShowJoinGamePanel()
    {
        yield return new WaitForSeconds(16.7f);
        JoinGamePanel.SetActive(true);
    }
}
