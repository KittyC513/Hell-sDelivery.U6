using UnityEngine;

public class SceneControl_BombDetonator : SceneControlBase<SceneControl_BombDetonator>
{
    public Transform[] spawnpoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "BombDetonator_testing";
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
