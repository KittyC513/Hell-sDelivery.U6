using UnityEngine;

public class SceneControl_Playtest1125 : SceneControlBase<SceneControl_Playtest1125>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform[] spawnPoints;
    void Start()
    {
        EventData.curSceneName = "Playtest1125";
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
            GameManager.instance.ResetPlayersPosition(spawnPoints[0], spawnPoints[1]);
            isResetPos = true;
        }

    }
}
