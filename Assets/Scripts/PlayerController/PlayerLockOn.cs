using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{
    private Camera playerCam; //the main camera attached to the player
    private bool isInPlayerCam = true; //if the camera is not on the regular player cam, the player cannot lock on
    [SerializeField] private GameObject playerObj;
    [SerializeField] private float detectionRadius = 8;
    [SerializeField] private LayerMask lockableLayerMask;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private PlayerInputDetection inputDetection;

    //debug variables
    private Vector3 lastRayStart;
    private Vector3 lastRayEnd;
    private Vector3 tempDir;

    private bool debug = true;


    // Update is called once per frame
    private void Awake()
    {
        if (playerController == null) playerController = playerObj.GetComponent<PlayerController>();
    }

    private void Start()
    {
        playerCam = inputDetection.cam;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject lockTarget = GetNewTarget(playerCam, playerObj);

        }
    }

    private GameObject GetNewTarget(Camera cam, GameObject player)
    {
        //consider changing this to OverlapSphereNonAlloc in the future
        Collider[] objectsInRange = Physics.OverlapSphere(player.transform.position, detectionRadius, lockableLayerMask);

        //if there are any colliders in the array
        if (objectsInRange.Length > 0)
        {
            float shortestDistance = 100;
            GameObject target = null;
            //check each object in range
            for (int i = 0; i < objectsInRange.Length; i++)
            {
                Vector3 objectPoint = objectsInRange[i].transform.position;
                Vector3 viewportPos = cam.WorldToViewportPoint(objectPoint);

                //if the object is in range of the viewport we can raycast towards it
                if (viewportPos.x < 1 && viewportPos.x > 0 && viewportPos.y < 1 && viewportPos.y > 0)
                {
                    Vector3 dir = (objectPoint - cam.transform.position).normalized;
                    tempDir = dir;
                    //send a raycast towards the target point
                    if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, 50))
                    {
                        if (hit.collider == objectsInRange[i])
                        {
                            //distance between the object and the centre of the camera
                            float dist = Mathf.Abs(viewportPos.x - 0.5f) + Mathf.Abs(viewportPos.y - 0.5f);

                            //if the distance to the object is shorter than the last make it the new shortest distance
                            if  (dist < shortestDistance)
                            {
                                shortestDistance = dist;
                                target = objectsInRange[i].gameObject;
                            }
                        }
                        
                    }
                }
            }


            if (debug)
            {
                lastRayStart = cam.transform.position;
                lastRayEnd = target.transform.position;
            }

            Debug.Log(cam.WorldToViewportPoint(target.transform.position));
            return target;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.DrawLine(lastRayStart, lastRayEnd);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(lastRayStart, tempDir);
        }
    }
}
