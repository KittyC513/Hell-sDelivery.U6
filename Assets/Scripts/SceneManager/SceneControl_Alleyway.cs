using UnityEngine;

public class SceneControl_Alleyway : SceneControlBase
{
    public Transform[] enterPoints;

    public EnterPlace enterPlace_Level1;
    public GameObject[] initialObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "Alleyway";
        ResetPlayerPos();

        //prevent the repeated gameobject creation
        if (EventData.firstTimeEnterScene)
        {
            for (int i = 0; i < initialObjects.Length; i++)
            {
                Destroy(initialObjects[i]);
            }
        }
        EventData.firstTimeEnterScene = true;
    }

    // Update is called once per frame
    void Update()
    {
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
        if (GameManager.instance.player1 == null || GameManager.instance.player2 == null) return;
        
        GameManager.instance.player1.transform.position = enterPoints[0].position;
        GameManager.instance.player2.transform.position = enterPoints[1].position;
    }



}
