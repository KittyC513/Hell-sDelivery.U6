using System.Collections.Generic;
using UnityEngine;

public class FollowerQueue : MonoBehaviour
{
    [SerializeField] private List<FollowerObject> followers;
    [SerializeField] private GameObject objectToFollow;
    [SerializeField] private GameObject emptyTransformObject;
    [HideInInspector] public List<GameObject> targetPositions;
    [SerializeField] private float pointDistanceT = 0.5f;
    private float t;
    private bool shouldUpdate;
    private Vector3 lastPos;

    private void Start()
    {
        //temporary debug only
        objectToFollow = GameManager.instance.player1;

        for (int i = 0; i < followers.Count; i++)
        {
            //create a bunch of follower points
            targetPositions.Add(Instantiate(emptyTransformObject, objectToFollow.transform.position, Quaternion.identity, this.transform));
        }
    }

    private void FixedUpdate()
    {
        
        t += Time.deltaTime;
        
        if (t>=pointDistanceT)
        {
            if ((objectToFollow.transform.position - lastPos).magnitude > 0.5f)
            {
               //update positions
                UpdatePoints();
                t = 0; 
                lastPos = objectToFollow.transform.position;
            }
            
        }
    }

    private void UpdatePoints()
    {
        for (int i = targetPositions.Count - 1; i > -1; i--)
        {
            if (i != 0)
            {
                targetPositions[i].transform.position = targetPositions[i - 1].transform.position;
                targetPositions[i].transform.rotation = targetPositions[i - 1].transform.rotation;
            }
            else
            {
                
                targetPositions[i].transform.position = objectToFollow.transform.position;
                targetPositions[i].transform.rotation = objectToFollow.transform.rotation;
                
            }

            followers[i].UpdatePosition(targetPositions[i].transform.position, targetPositions[i].transform.rotation);
        }
    }
}
