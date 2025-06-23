using UnityEngine;

public class SceneControl_PostOffice : MonoBehaviour
{
    public Transform enterPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

    }
    void Start()
    {
        //reset players position
        GameManager.instance.player1.transform.position = enterPoint.position;
        GameManager.instance.player2.transform.position = enterPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
