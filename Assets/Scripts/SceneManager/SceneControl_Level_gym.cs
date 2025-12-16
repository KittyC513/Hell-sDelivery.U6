using UnityEngine;

public class SceneControl_LevelGym : SceneControlBase<SceneControl_LevelGym>
{
    public Transform[] spawnpoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "Level_gym";
    }

    // Update is called once per frame
    void Update()
    {
        ResetPlayerPos();
    }

    private void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            GameManager.instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
            isResetPos = true;
        }

    }
}
