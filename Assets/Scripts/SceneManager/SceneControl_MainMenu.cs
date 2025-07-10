using UnityEngine;

public class SceneControl_MainMenu : SceneControlBase<SceneControl_MainMenu>
{
    public GameObject characterSelectPanel;
    private void Start()
    {
        EventData.curSceneName = "StartScene";
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isOnCharacterSelection)
        {
            characterSelectPanel.SetActive(true);
        }
    }
}
