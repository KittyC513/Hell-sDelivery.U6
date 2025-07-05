using System.Drawing.Printing;
using UnityEngine;
using Yarn.Unity.Editor;

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
    private int pointIndex = 0;
    private float t = 0;
    private int dir = 1;
    private bool stall = false;
    private float stallTime = 0;
    private float stallTemp = 0;

    public enum PlatformType { boomerang, continuous }
    [SerializeField] public PlatformType platformType;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetNextPosition();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.MovePosition(currentPos);

        if (!stall)
        {
            t += Time.fixedDeltaTime * moveSpeed;
        }
        else
        {
            stallTemp += Time.fixedDeltaTime;

            if (stallTemp >= stallTime)
            {
                stall = false;
                stallTemp = 0;
            }
        }
     
        currentPos = Vector3.Lerp(startPos, nextPos, t);

        if (t >= 1)
        {
            SetNextPosition();
        }
    }

    private void SetNextPosition()
    {
        startPos = points[pointIndex].position;

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
        }

        nextPos = points[pointIndex].position;
        t = 0;
    }
}
