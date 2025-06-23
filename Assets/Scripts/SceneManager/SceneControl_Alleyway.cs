using UnityEngine;

public class SceneControl_Alleyway : SceneControl_Base
{
    public Transform[] enterPoints;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(GameManager.instance.player1 != null && GameManager.instance.player2 != null)
        {
            GameManager.instance.player1.transform.position = enterPoints[0].position;
            GameManager.instance.player2.transform.position = enterPoints[1].position;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
