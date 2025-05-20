using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{
    private Camera playerCam; //the main camera attached to the player
    //private bool isInPlayerCam = true; //if the camera is not on the regular player cam, the player cannot lock on
    [SerializeField] private GameObject playerObj;
    [SerializeField] private float detectionRadius = 8;
    [SerializeField] private LayerMask lockableLayerMask;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private PlayerInputDetection inputDetection;
    public CameraManager CameraManager;

    //debug variables
    private Vector3 lastRayStart;
    private Vector3 lastRayEnd;
    private Vector3 tempDir;

    private bool debug = false;

    public GameObject lockTarget;
    [HideInInspector] public bool isLockedOn = false;


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
        if (DetectLockInput())
        {  
            
            if ((lockTarget != null))
            {

                CameraManager.currentCamType = E_CamType.lockCam;
                print("Lock_on" + lockTarget.transform.position);
                if (!isLockedOn)
                {
                    CameraManager.ResetCamTransition();
                    playerController.isLookAtTriggered = false;
                    isLockedOn = true;
                }

            }
            else
            {
                lockTarget = GetNewTarget(playerCam, playerObj);
                
            }

        }
        else
        {

            lockTarget = null;
            CameraManager.currentCamType = E_CamType.playerCam;
            isLockedOn = false;
        }

        if (lockTarget != null && DetectLockInput())
        {
            Vector2 direction = inputDetection.GetHorizontalMovement();
        }
    }

    public bool DetectLockInput()
    {
        return inputDetection.lockPressed;
    }


    public GameObject GetNewTarget(Camera cam, GameObject player)
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

            //Debug.Log(cam.WorldToViewportPoint(target.transform.position));
            return target;
        }

        return null;
    }

    public GameObject GetNextTarget(Vector2 direction, Camera cam, GameObject player)
    {
        //get all visible targets
        //get each ones position on the view port
        //get the target closest in the dir direction

        //calculate the viewport angle between each object and the lock target
        //check each object that has an angle smaller than 90 (this means nothing negative will be returned)
        //get the object that has the shortest distance away from this object

        //loop through each available target that is to the right of the origin target
        //check the distances until you get the shortest distance in that direction


        //consider changing this to OverlapSphereNonAlloc in the future

        Collider[] objectsInRange = Physics.OverlapSphere(player.transform.position, detectionRadius, lockableLayerMask);
        
        if (lockTarget != null)
        {
            //if there are any colliders in the array
            if (objectsInRange.Length > 0)
            {
                float shortestDistance = 100;
                GameObject target = null;

                //check each object in range
                for (int i = 0; i < objectsInRange.Length; i++)
                {
                    //the position of the object
                    Vector3 objectPoint = objectsInRange[i].transform.position;

                    //the position of the object on the viewport
                    Vector3 viewportPos = cam.WorldToViewportPoint(objectPoint);

                    //if the object is in range of the viewport we can raycast towards it
                    if (viewportPos.x < 1 && viewportPos.x > 0 && viewportPos.y < 1 && viewportPos.y > 0)
                    {
                        //if the angle between the input direction and the next object in array is less than 90 consider it as a new target
                        if (Vector2.Angle(direction, viewportPos) < 90 && objectsInRange[i] != lockTarget)
                        {
                            //direction towards the object from the camera
                            Vector3 dir = (objectPoint - cam.transform.position).normalized;

                            //send a raycast towards the target point, if it hits we can see the object
                            if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, 50))
                            {
                                //if the raycast hits the correct object
                                if (hit.collider == objectsInRange[i])
                                {
                                    //the position of the current lock target in the viewport
                                    Vector3 lockTargetViewport = cam.WorldToViewportPoint(lockTarget.transform.position);

                                    //distance between the object and the lock target
                                    float dist = Vector2.Distance(lockTargetViewport, viewportPos);

                                    //if the distance to the object is shorter than the last make it the new shortest distance
                                    if (dist < shortestDistance)
                                    {
                                        shortestDistance = dist;
                                        target = objectsInRange[i].gameObject;
                                    }
                                }

                            }
                        }
                        
                    }
                }
                return target;
            }
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
