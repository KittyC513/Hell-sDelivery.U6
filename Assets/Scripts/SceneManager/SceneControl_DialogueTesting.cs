using UnityEngine;

public class SceneControl_DialogueTesting : SceneControlBase<SceneControl_PostOffice>
{
    public Transform[] spawnpoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "DialogueTesting";
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
}
