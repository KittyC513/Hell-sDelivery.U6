using UnityEngine;

public class CameraMovement_Level : MonoBehaviour
{
    [HideInInspector]
    public Transform camTransform;

    public float offSet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camTransform = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FollowPlayerMovement(Transform player)
    {
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, player.position.z - offSet);
        Debug.Log("Camera position: " + this.gameObject.transform.position);
    }
}
