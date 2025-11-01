using UnityEngine;

public class SceneControl_Alleyway : SceneControlBase<SceneControl_Alleyway>
{
    public Transform[] spawnpoints;

    public EnterPlace enterPlace_Level1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "Alleyway";
        GameManager.instance.FreezeBothPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        ResetPlayerPos();
        if (EventData.isAcceptedMission_lalah && !enterPlace_Level1.enabled)
        {
            enterPlace_Level1.enabled = true;
        }
        else if (!EventData.isAcceptedMission_lalah && enterPlace_Level1.enabled)
        {
            enterPlace_Level1.enabled = false;
        }
    }

    public void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            GameManager.instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
            isResetPos = true;
        }
    }

    public void UnfreezePlayers()
    {
        GameManager.instance.UnFreezeBothPlayers();
        Camera.main.gameObject.SetActive(false);
    }
}
