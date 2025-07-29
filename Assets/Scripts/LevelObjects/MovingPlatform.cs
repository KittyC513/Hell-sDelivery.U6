using Unity.VisualScripting;
using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 nextPos;
    private Vector3 startPos;
    private Vector3 currentPos;

    [SerializeField] private Transform[] points;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pointStallTime = 0.1f;
    [SerializeField] private float endPointStallTime = 1;

    public bool isActive = true;
    private bool reachedDestiation = false;
    public enum EaseType { None, InQuad, InQuart, InOutSine, OutQuad }
    [SerializeField] public EaseType easeType = EaseType.InQuad;

    private int pointIndex = 0;
    private float t = 0;
    private int dir = 1;
    private bool stall = false;
    private float stallTime = 0;
    private float stallTemp = 0;

    private float distance;

    //easing functions used to determine the platforms smooth movement
    public static float InQuad(float t) => t * t;
    public static float InQuart(float t) => t * t * t * t;
    public static float InOutSine(float t) => (float)(Mathf.Cos(t * Mathf.PI) - 1) / -2;
    public static float OutQuad(float t) => 1 - InQuad(1 - t);

    public enum PlatformType { boomerang, continuous, oneWay }
    [SerializeField] public PlatformType platformType;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetNextPosition();

        //update the position of the object
        transform.position = points[0].position;

    }

    private void FixedUpdate()
    {
        if (isActive && !reachedDestiation)
        {
            MovePlatform();
        }
    }

    public void ActivatePlatform()
    {
        if (!isActive)
        {
            isActive = true;
        }
      
    }

    private void MovePlatform()
    {
        //if not stalling update the time value
        if (!stall)
        {
            t += Time.fixedDeltaTime * moveSpeed;
        }
        else //otherwise count down until the stall ends
        {
            stallTemp += Time.fixedDeltaTime;

            if (stallTemp >= stallTime)
            {
                stall = false;
                stallTemp = 0;
            }
        }

        float percent = t / distance;

        float easedTime = percent;

        //use the right easing function 
        switch (easeType)
        {
            case EaseType.None:
                easedTime = percent;
                break;
            case EaseType.InQuad:
                easedTime = InQuad(percent);
                break;
            case EaseType.InQuart:
                easedTime = InQuart(percent);
                break;
            case EaseType.InOutSine:
                easedTime = InOutSine(percent);
                break;
            case EaseType.OutQuad:
                easedTime = OutQuad(percent);
                break;
        }

        //lerp the position using the eased percent time
        currentPos = Vector3.Lerp(startPos, nextPos, easedTime);

        //if the time has reached the end set the new position
        if (t >= distance)
        {
            SetNextPosition();
        }

        //update the position of the object
        transform.position = currentPos;
    }

    private void SetNextPosition()
    {
        //set the new start position to the current position (object is at this position now)
        startPos = points[pointIndex].position;

        //select next point based on the style of platform
        //boomerang goes back and forth, coninuous loops back around to the first point 
        switch (platformType)
        {
            case PlatformType.boomerang:
                if (pointIndex + (1 * dir) > points.Length - 1 ||
                    pointIndex + (1*dir) < 0)
                {
                    dir *= -1;
                    pointIndex += 1 * dir;

                    //start stalling
                    stallTime = endPointStallTime;
                    stall = true;
                    
                }
                else
                {
                    pointIndex += 1 * dir;

                    //start stalling
                    stallTime = pointStallTime;
                    stall = true;
                  
                }
                break;
            case PlatformType.continuous:
                if (pointIndex + (1 * dir) > points.Length - 1)
                {
                    pointIndex = 0;

                    //start stalling
                    stallTime = endPointStallTime;
                    stall = true;
                   
                }
                else
                {
                    pointIndex += 1 * dir;

                    //start stalling
                    stallTime = pointStallTime;
                    stall = true;
                   
                }
                break;
            case PlatformType.oneWay:
                if (pointIndex + (1 * dir) > points.Length - 1)
                {
                    reachedDestiation = true;
                }
                else
                {
                    pointIndex += 1 * dir;

                    //start stalling
                    stallTime = pointStallTime;
                    stall = true;
                }
                break;
        }

        nextPos = points[pointIndex].position;
        distance = Vector3.Distance(startPos, nextPos);
        t = 0;
    }
}
