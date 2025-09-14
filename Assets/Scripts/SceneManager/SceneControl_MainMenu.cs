using System.Collections;
using UnityEngine;

public class SceneControl_MainMenu : SceneControlBase<SceneControl_MainMenu>
{
    public GameObject characterSelectPanel;
    public GameObject JoinGamePanel;
    private void Start()
    {
        JoinGamePanel.SetActive(false);
        EventData.curSceneName = "StartScene";
        StartCoroutine(ShowJoinGamePanel());
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isOnCharacterSelection)
        {
            characterSelectPanel.SetActive(true);
        }
    }

    IEnumerator ShowJoinGamePanel()
    {
        yield return new WaitForSeconds(10.2f);
        JoinGamePanel.SetActive(true);
    }
}
