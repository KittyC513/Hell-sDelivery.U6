using UnityEngine;

public class FollowerObject : MonoBehaviour
{
    [HideInInspector] public Vector3 nextTargetPosition;
    private Quaternion nextTargetRotation;
    private Vector3 lastPos;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed = 50;
    private float t;

    private void Update()
    {
       
        t += moveSpeed * Time.deltaTime;
        //t = Mathf.Clamp(t, 0, 1);
        float percent = t / 1;
        
        
        transform.position = Vector3.Lerp(transform.position, nextTargetPosition, percent);
    
        transform.rotation = Quaternion.Slerp(transform.rotation, nextTargetRotation, rotateSpeed * Time.deltaTime); 

    }

    public void UpdatePosition(Vector3 nextPos, Quaternion nextRotation)
    {
        
        //Debug.Log("Start Pos" + transform.position);

        lastPos = transform.position;
        
        nextTargetPosition = nextPos;
        nextTargetRotation = nextRotation;
        t = 0;
    }

    
}
