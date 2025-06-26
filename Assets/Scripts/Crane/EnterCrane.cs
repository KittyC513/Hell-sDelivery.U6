using UnityEngine;

public class EnterCrane : MonoBehaviour
{
    public bool p1EnterCrane = false;
    public bool p2EnterCrane = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player1"))
            {
                p1EnterCrane = true;
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Player2"))
            {
                p2EnterCrane = true;
            }
            EventData.craneIsActivated = true;
        }
    }
}
