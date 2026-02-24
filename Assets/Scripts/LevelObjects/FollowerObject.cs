using UnityEngine;

public class FollowerObject : MonoBehaviour
{
    [HideInInspector] public Vector3 nextTargetPosition;
    public Quaternion lastRotation;
    private Quaternion nextTargetRotation;
    private Vector3 lastPos;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed = 50;
    private float t;
    [SerializeField] private bool active;

    public void ToggleActive(bool state)
    {
        active = state;
    }

    private void Update()
    {
        if (active)
        {
             t += moveSpeed * Time.deltaTime;
            //t = Mathf.Clamp(t, 0, 1);
            float percent = t / 1;
            
            
            transform.position = Vector3.Lerp(lastPos, nextTargetPosition, percent);
        
            transform.rotation = Quaternion.Slerp(lastRotation, nextTargetRotation, rotateSpeed * Time.deltaTime); 
        }
    }

    public void UpdatePosition(Vector3 nextPos, Quaternion nextRotation)
    {
        
        //Debug.Log("Start Pos" + transform.position);

        lastPos = transform.position;
        lastRotation = transform.rotation;
        
        nextTargetPosition = nextPos;
        nextTargetRotation = nextRotation;
        t = 0;
    }

    
}
