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

    private bool canSwitchTarget = true;


    // Update is called once per frame
    private void Awake()
    {
        if (playerController == null) playerController = playerObj.GetComponent<PlayerController>();
    }

    private void Start()
    {
        

    }

    void Update()
    {
        playerCam = inputDetection.cam;
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

        if (lockTarget != null && DetectLockInput() && inputDetection.GetCameraMovement() != Vector2.zero && canSwitchTarget)
        {
            Vector2 direction = inputDetection.GetCameraMovement().normalized;
            lockTarget = GetNextTarget(direction, playerCam, playerObj);
            canSwitchTarget = false;
        }

        if (inputDetection.GetCameraMovement().magnitude == 0)
        {
            canSwitchTarget = true;
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

    public GameObject GetNextTarget(Vector3 inputDir, Camera cam, GameObject player)
    {
        Collider[] objectsInRange = Physics.OverlapSphere(player.transform.position, detectionRadius, lockableLayerMask);
        
        if (lockTarget != null)
        {
            //if there are any colliders in the array
            if (objectsInRange.Length > 0)
            {
                float shortestDistance = 100;
                GameObject target = lockTarget;

                //check each object in range
                for (int i = 0; i < objectsInRange.Length; i++)
                {
                    //the position of the current lock target in the viewport
                    Vector3 lockTargetViewport = cam.WorldToViewportPoint(lockTarget.transform.position);
                    //the position of the object
                    Vector3 objectPoint = objectsInRange[i].transform.position;

                    //the position of the object on the viewport
                    Vector3 viewportPos = cam.WorldToViewportPoint(objectPoint);
                    
                    //if the object is in the viewport
                    if (viewportPos.x < 1 && viewportPos.x > 0 && viewportPos.y < 1 && viewportPos.y > 0)
                    {
                        //the direction towards the next object
                        Vector3 toDirection = (viewportPos - lockTargetViewport).normalized;

                        //if the direction towards the next target and the direction the player is inputting match up its in the correct direction
                        if (toDirection.x > 0 && inputDir.x > 0 || toDirection.x < 0 && inputDir.x < 0)
                        {
                            //if the angle between the input direction and the next object in array is less than 90 consider it as a new target
                            if (objectsInRange[i].gameObject != lockTarget)
                            {
                                //direction towards the object from the camera
                                Vector3 dir = (objectPoint - cam.transform.position).normalized;

                                //send a raycast towards the target point, if it hits we can see the object
                                if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, 50))
                                {
                                    //if the raycast hits the correct object
                                    if (hit.collider == objectsInRange[i])
                                    {
                                        //just use the horizontal position of both objects (you can lock onto objects higher up if its closer horizontally
                                        Vector3 lockPoint = new Vector3(lockTargetViewport.x, 0, 0);
                                        Vector3 targetPoint = new Vector3(viewportPos.x, 0, 0);

                                        //distance between the object and the lock target
                                        float dist = Vector2.Distance(lockPoint, targetPoint);

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
                }
                return target;
            }
        }



        return lockTarget;
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
