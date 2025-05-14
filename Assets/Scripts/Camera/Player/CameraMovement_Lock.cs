using UnityEngine;

public class CameraMovement_Lock : MonoBehaviour
{
    public Transform player;
    private Vector3 afterPos;
    public Vector3 offSet;

    public bool isLockTrigger = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            isLockTrigger = true;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        afterPos = player.position + offSet;

        this.transform.position = afterPos;
        this.transform.LookAt(player);

        if(isLockTrigger)
        {

        }
    }
}
