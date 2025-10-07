using UnityEngine;

public class SceneControl_DialogueTesting : SceneControlBase<SceneControl_PostOffice>
{
    public Transform[] enterPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "DialogueTesting";
        //reset players position
        ResetPlayerPos();
    }

    // Update is called once per frame
    void Update()
    {
        
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
