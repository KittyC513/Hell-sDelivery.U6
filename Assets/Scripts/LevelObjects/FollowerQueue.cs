using System.Collections.Generic;
using UnityEngine;

public class FollowerQueue : MonoBehaviour
{
    [SerializeField] public List<FollowerObject> followers;
    [SerializeField] private GameObject objectToFollow;
    [SerializeField] private GameObject emptyTransformObject;
    
    [HideInInspector] public List<GameObject> targetPositions;
    [SerializeField] private float pointDistanceT = 0.5f;
    [SerializeField] private int starterFollowPoints = 3;
    [SerializeField] private Transform parent;
    private float t;
    private bool shouldUpdate;
    private Vector3 lastPos;

    private void Start()
    {
        //temporary debug only
        //objectToFollow = GameManager.instance.player1;

        for (int i = 0; i < starterFollowPoints; i++)
        {
            //create some follower points
            targetPositions.Add(Instantiate(emptyTransformObject, objectToFollow.transform.position, Quaternion.identity, parent));
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

            if (i < followers.Count)
            {
                if (followers[i] != null)
                {
                    followers[i].UpdatePosition(targetPositions[i].transform.position, targetPositions[i].transform.rotation);
                }
                else if (i != 0)
                {
                    followers.RemoveAt(i);
                }
            }
        }
    }

    private void UpdateFollowPositions()
    {
        for (int i = targetPositions.Count - 1; i > -1; i--)
        {
            if (i < followers.Count)
            {
                if (followers[i] != null)
                {
                    followers[i].UpdatePosition(targetPositions[i].transform.position, targetPositions[i].transform.rotation);
                }
                else if (i != 0)
                {
                    followers.RemoveAt(i);
                }
            }
        }
    }

    public void AddNewFollower(FollowerObject follower)
    {
        if (followers.Count + 1 > targetPositions.Count)
        {
            targetPositions.Add(Instantiate(emptyTransformObject, objectToFollow.transform.position, Quaternion.identity, parent));
        }
        followers.Add(follower);

        //UpdateFollowPositions();
    }

    public void RemoveFollower(FollowerObject follower)
    {
        followers.Remove(follower);

        //UpdateFollowPositions();
    }

    public FollowerObject DoesQueueContainTag(string tag)
    {
        for (int i = 0; i < followers.Count; i++)
        {
            if (followers[i] != null)
            {
                if (followers[i].CompareTag(tag))
                {
                    return followers[i];
                }
            }
           
        }

        return null;
    }
}
