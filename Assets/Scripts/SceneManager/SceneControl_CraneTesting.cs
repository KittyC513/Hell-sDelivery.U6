using UnityEngine;

public class SceneControl_CraneTestinhg : SceneControlBase<SceneControl_CraneTestinhg>
{
    public Transform[] spawnpoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "CraneTesting";
        EventData.craneIsActivated = false;
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
