using UnityEngine;

public class SceneControl_Minigames : SceneControlBase<SceneControl_Minigames>
{
    public Transform[] spawnPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "minigame";

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
            GameManager.Instance.ResetPlayersPosition(spawnPoints[0], spawnPoints[1]);
            isResetPos = true;
        }
    }
}
