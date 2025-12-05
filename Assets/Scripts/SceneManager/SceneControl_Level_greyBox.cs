using UnityEngine;

public class SceneControl_Level_greyBox : SceneControlBase<SceneControl_Level_greyBox>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform[] spawnPoints;

    void Start()
    {
        EventData.curSceneName = "Level_greyBox";
    }

    // Update is called once per frame
    void Update()
    {
        ResetPlayerPos();
    }

    void ResetPlayerPos()
    {
        if(!isResetPos)
        {
            GameManager.Instance.ResetPlayersPosition(spawnPoints[0], spawnPoints[1]);
            isResetPos = true;
        }
    }
}
