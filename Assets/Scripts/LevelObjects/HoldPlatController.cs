using System.Collections.Generic;
using UnityEngine;

public class HoldPlatController : MonoBehaviour
{
    private HoldPlatform parentPlatform;
    private bool active = false;
    private float moveTimer = 0;
    private float moveSpeed = 1;

    private float sinTimer = 0;

    private bool initialized = false;

    private Vector3 oldPosition;
    private float distanceBetween;
    private List<Transform> points;

    private Vector3 sineChange;
    private float frequency;
    private float sinHeight;

    private Vector3 startPos;
    private Vector3 endPos;
    private float randomSinOffset;

    public void InitializePlatform(HoldPlatform plat, float _moveSpeed, List<Transform> _points, float _frequency, float _sinHeight)
    {
        //get the list of points
        points = new List<Transform>();
        points = _points;

        //get the distance between the starting position and the end position
        distanceBetween = Vector3.Distance(points[0].position, points[1].position);

        //grab values from the parent object
        parentPlatform = plat;
        moveSpeed = _moveSpeed;
        frequency = _frequency;
        sinHeight = _sinHeight;

        //set a start and end position that maintains the x and z of this platform
        startPos = new Vector3(transform.position.x, points[0].position.y, transform.position.z);
        endPos = new Vector3(transform.position.x, points[1].position.y, transform.position.z);

        //setup the starting position
        transform.position = startPos;
        oldPosition = transform.position;

        //apply a random offset to the sin wave to desync all the platforms
        randomSinOffset = Random.Range(0, 10);

        initialized = true;
    }

    public void FixedUpdate()
    {
        if (initialized) MovePlatformToDestination();
    }

    public void MovePlatformToDestination()
    {
        //percentage used for the lerp is move timer over the distance between
        //this is so that the platform can move at a constant rate from point a to b
        float percent = moveTimer / distanceBetween;

        if (moveTimer < distanceBetween + 0.1f)
        {
            moveTimer += Time.deltaTime * moveSpeed;
        }

        //active determines whether the platforms should be going towards the end position or start position
        if (active)
        {
            //the platform has reached the final position
            if (percent >= 0.99f)
            {
                //this will smoothly move from the final position to the sin wave offset to avoid teleporting
                if (sinTimer < 1)
                {
                    sinTimer += Time.deltaTime;
                }

                //apply the sin wave bob to the platform
                Vector3 targetPos = ApplySineWave(frequency, sinHeight, endPos);

                //move from current position to targetPos
                transform.position = Vector3.Lerp(endPos, targetPos, sinTimer / 1);
                
            }
            else
            {
                //move from the current position to the target position
                sinTimer = 0;
                transform.position = Vector3.Lerp(oldPosition, endPos, percent);
            }
        }
        else
        {
            //move from the current position to the original position
            transform.position = Vector3.Lerp(oldPosition, startPos, percent);
        }
    }

    //called when the platforms are activated
    public void OnPlatformActivate()
    {
        oldPosition = transform.position;

        distanceBetween = Vector3.Distance(oldPosition, endPos);

        moveTimer = 0;
        active = true;
        
    }
    
    //called when the platforms are deactivated
    public void OnPlatformDeactivate()
    {
        oldPosition = transform.position;

        distanceBetween = Vector3.Distance(oldPosition, startPos);

        moveTimer = 0;

        active = false;
        
    }

    private Vector3 ApplySineWave(float frequency, float amplitude, Vector3 _startPos)
    {
        float yPos = amplitude * Mathf.Sin((Time.time+ randomSinOffset) * frequency);

        return new Vector3(_startPos.x, _startPos.y + yPos, _startPos.z);
    }


}
