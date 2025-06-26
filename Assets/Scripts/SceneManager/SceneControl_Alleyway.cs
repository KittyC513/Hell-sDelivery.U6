using UnityEngine;

public class SceneControl_Alleyway : SceneControlBase
{
    public Transform[] enterPoints;

    public EnterPlace enterPlace_Level1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "Alleyway";


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
            GameManager.instance.player1.transform.position = enterPoints[0].position;
            GameManager.instance.player2.transform.position = enterPoints[1].position;
            isResetPos = true;
        }

    }



}
