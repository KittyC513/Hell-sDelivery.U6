using UnityEngine;

public class SceneControl_PostOffice : MonoBehaviour
{
    public Transform enterPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        //reset players position
        PlayerManager.Instance.players[0].transform.position = enterPoint.position;
        PlayerManager.Instance.players[1].transform.position = enterPoint.position;
    }
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
