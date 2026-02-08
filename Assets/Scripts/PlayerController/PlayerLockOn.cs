using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum E_LockOnState
{
    none,
    bombLockOn,
    detonatorLockOn,
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]



public class PlayerLockOn : MonoBehaviour
{
    private Camera playerCam; //the main camera attached to the player
    //private bool isInPlayerCam = true; //if the camera is not on the regular player cam, the player cannot lock on
    [SerializeField] private GameObject playerObj;
    [SerializeField] private float detectionRadius = 8;
    [SerializeField] private LayerMask lockableLayerMask;
    [SerializeField] private LayerMask detectableLayers;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private PlayerInputDetection inputDetection;
    public CameraManager CameraManager;
    public CameraMovement_Player cameraMovement_player;

    //debug variables
    private Vector3 lastRayStart;
    private Vector3 lastRayEnd;
    private Vector3 tempDir;

    private bool debug = false;

    public GameObject lockTarget;
    [HideInInspector] public bool isLockedOn = false;

    private bool canSwitchTarget = true;

    [Header("Detonator LockOn")]
    public float viewAngle = 90f;
    public float viewRadius = 15f;

    public LayerMask obstacleMask;
    public LayerMask targetMask;

    public bool isWithDetonator = false;
    public bool isWithBomb = false;
    public bool isDetonatorLockOn = false;
    public bool isBombLockOn = false;
    public bool canSeeTarget = true;   

    public List<Transform> visibleTargets = new List<Transform>();

    public E_LockOnState lockCamType = E_LockOnState.none;

    [Header("Cone Mesh Generator")]
    public int resolution = 40;
    Mesh mesh;


    // Update is called once per frame
    private void Awake()
    {
        if (playerController == null) playerController = playerObj.GetComponent<PlayerController>();
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

    }

    void Update()
    {

        playerCam = inputDetection.cam;
        if (DetectLockInput())
        {
            if(!isWithDetonator && !isWithBomb)
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
            else if (isWithDetonator)
            {
                lockCamType = E_LockOnState.detonatorLockOn;
                CameraManager.currentCamType = E_CamType.playerCam; 
                cameraMovement_player.camMode = E_CamMode.coneSight;
                ConeSightDetection();
                isDetonatorLockOn = true;
                //if (!isLockedOn)
                //{
                //    CameraManager.ResetCamTransition();
                //    playerController.isLookAtTriggered = false;
                //    isLockedOn = true;
                //}
            }
            else if(isWithBomb)
            {
                lockCamType = E_LockOnState.bombLockOn;
                CameraManager.currentCamType = E_CamType.playerCam;
                cameraMovement_player.camMode = E_CamMode.bombThrowingCam;
                isBombLockOn = true;
            }
            //else
            //{

            //    if ((lockTarget != null))
            //    {

            //        CameraManager.currentCamType = E_CamType.lockCam;
            //        print("Lock_on" + lockTarget.transform.position);
            //        if (!isLockedOn)
            //        {
            //            CameraManager.ResetCamTransition();
            //            playerController.isLookAtTriggered = false;
            //            isLockedOn = true;
            //        }

            //    }
            //    else
            //    {
            //        lockTarget = GetNewTarget(playerCam, playerObj);

            //    }

            //}         
        }
        else
        {
            if(isDetonatorLockOn) 
                isDetonatorLockOn = false;
            if(isBombLockOn) 
                isBombLockOn = false;
        }

        //if (isBombLockOn)
        //{
        //    isBombLockOn = true;
        //    lockCamType = E_LockOnState.bombLockOn;
        //    CameraManager.currentCamType = E_CamType.playerCam;
        //    cameraMovement_player.camMode = E_CamMode.bombThrowingCam;
        //}


        if (!DetectLockInput())
        {
            lockCamType = E_LockOnState.none;
            // Turn OFF previous selected targets
            for (int i = 0; i < visibleTargets.Count; i++)
            {
                if (visibleTargets[i] == null) continue;
                var bm = visibleTargets[i].GetComponent<BombMovement>();
                if (bm != null) bm.isOnSelected = false;
            }

            //clear the list of visible targets
            visibleTargets.Clear();

            lockTarget = null;
            CameraManager.currentCamType = E_CamType.playerCam;
            isLockedOn = false;
            isDetonatorLockOn = false;

            visibleTargets.Clear();
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

    private void LateUpdate()
    {
        if(DetectLockInput() && isWithDetonator)
            GenerateConeMesh();
        else
            mesh.Clear();
    }

    public void GenerateConeMesh()
    {

        int vertexCount = resolution + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(resolution) * 3];

        // origin
        vertices[0] = Vector3.zero;

        float step = viewAngle / resolution;
        float half = viewAngle / 2f;

        // generate boundary vertices
        for (int i = 0; i <= resolution; i++)
        {
            float angle = -half + step * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
            vertices[i + 1] = dir * viewRadius;
        }

        // triangles
        int triIndex = 0;
        for (int i = 0; i < resolution; i++)
        {
            triangles[triIndex++] = 0;
            triangles[triIndex++] = i + 1;
            triangles[triIndex++] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }

    public bool DetectLockInput()
    {
        return inputDetection.lockPressed;
    }

    public bool DetectThrowInput()
    {
        return inputDetection.crouchPressed;
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
                    if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, 50, detectableLayers))
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
                                if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, 50, detectableLayers))
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

    public void ConeSightDetection()
    {
        // Turn OFF previous selected targets
        for (int i = 0; i < visibleTargets.Count; i++)
        {
            if (visibleTargets[i] == null) continue;
            var bm = visibleTargets[i].GetComponent<BombMovement>();
            if (bm != null) bm.isOnSelected = false;
        }

        //clear the list of visible targets
        visibleTargets.Clear();

        //1. find all colliders within view radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        float cosHalfFov = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);

        foreach (Collider col in targetsInViewRadius)
        {
            Transform target = col.transform;

            Vector3 dirToTarget = target.position - transform.position;
            float disToTarget = dirToTarget.magnitude;

            Vector3 dirNormalized = dirToTarget.normalized;

            //2. check if target is within view angle
            float dot = Vector3.Dot(transform.forward, dirNormalized);
            float angleToTarget = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);


            if (dot < cosHalfFov)
            {
                continue;
            }

            //3. check for obstacles between the player and target
            if (Physics.Raycast(transform.position, dirNormalized, out RaycastHit hit, disToTarget, obstacleMask))
            {
                //Something in the way
                continue;

            }

            visibleTargets.Add(target);
            //apply bomb selected visual effect
            BombMovement bombMovement = target.GetComponent<BombMovement>();
            if (bombMovement != null)
                bombMovement.isOnSelected = true;

            //for(int i = 0; i < visibleTargets.Count; i++)
            //{
            //    if (!targetsInViewRadius.Contains(visibleTargets[i].GetComponent<Collider>()))
            //    {
            //        BombMovement bomb = visibleTargets[i].GetComponent<BombMovement>();
            //        bomb.isOnSelected = false;
            //        visibleTargets.RemoveAt(i);
            //    }
            //}
            BombMovement bomb = target.GetComponent<BombMovement>();
            if (bomb != null) bomb.isOnSelected = true;

        }

        canSeeTarget = visibleTargets.Count > 0;
    }
   
    private void OnDrawGizmos()
    {
        //if (debug)
        //{
        //    Gizmos.DrawLine(lastRayStart, lastRayEnd);

        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawRay(lastRayStart, tempDir);
        //}

        // 1. Draw view radius
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // 2. Draw cone edges
        Vector3 leftDir = DirFromAngle(-viewAngle / 2f);
        Vector3 rightDir = DirFromAngle(viewAngle / 2f);

        Gizmos.DrawLine(transform.position, transform.position + leftDir * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * viewRadius);

        // 3. Draw line to target (if assigned)

        //if(visibleTargets.Count != 0)
        //{
        //    foreach (Transform target in visibleTargets)
        //    {
        //        Gizmos.color = canSeeTarget ? Color.cyan : Color.gray;
        //        Gizmos.DrawLine(transform.position, target.position);
        //    }
        //}
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        float rad = (angleInDegrees + transform.eulerAngles.y) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }
}

