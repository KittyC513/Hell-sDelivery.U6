using UnityEngine;

public class CameraMovement_Level : MonoBehaviour
{
    [HideInInspector]
    public Transform camTransform;

    public float offSet;

    public float offSetX;
    public float offSetY;
    private Vector3 movePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camTransform = this.gameObject.transform;
    }


    public void FollowPlayerMovement(Transform player)
    {
        movePos.x = this.gameObject.transform.position.x - offSetX;
        movePos.y = this.gameObject.transform.position.y - offSetY;
        movePos.z = player.position.z - offSet;
        this.gameObject.transform.position = movePos;
        Debug.Log("Camera position: " + this.gameObject.transform.position);
    }
}
