using UnityEngine;

public class CameraMovement_Scene : MonoBehaviour
{
    public Transform p1Pos;
    public Transform p2Pos;
    public Vector3 midPoint;

    public float lerpSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Movement();
    }

    void Movement()
    {
        if (p1Pos != null && p2Pos != null)
        {         
            midPoint = new Vector3((p1Pos.position.x + p2Pos.position.x) / 2, this.transform.position.y, this.transform.position.z);
            transform.position = Vector3.Lerp(this.transform.position, midPoint,Time.deltaTime * lerpSpeed);
        }
    }
}
