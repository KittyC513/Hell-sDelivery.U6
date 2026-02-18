using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SceneControl_Level_greyBox : SceneControlBase<SceneControl_Level_greyBox>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform[] spawnPoints;
    public DialogueSystemTrigger dialogueSystem_devil;
    public DialogueSystemController dialogueSystemController;

    [Header("Crane Control")]
    public Transform craneArm;
    public Transform craneTrolley;
    public Transform craneSurface;
    public EnterCrane enterCrane;
    public PlayerInputDetection playerInput;
    public float craneMoveSpeed = 5f;
    public float craneRotateSpeed = 20f;

    //constraints for crane movement
    public Vector3 craneMinMaxHight;
    public Vector3 magnetMinMaxDistance;

    //surface size for magnet detection
    public float surfaceSizeY;
    public Vector3 surfaceSize;
    public Vector3 surfaceCenter;
    public Vector3 offset;
    public Transform childObjs;
    public float magneticForce = 10f;

    public Camera cam_controlRoom;
    public float camSwitchDelay = 0.5f;
    public float camSwitchTimer = 0f;

    public Collider magneticSurfaceCollider;
    Coroutine magnetCoroutine;
    Transform magneticObj;

    [Header("Cutscene Cotrol")]
    public GameObject cutscene_overview;
    public GameObject cutscene_intro;
    public GameObject cutscene_bombLoserIntro;
    public GameObject cutscene_gateExplosion;
    public GameObject cutscene_hellHoundFight;
    public GameObject cutscene_RVSquatterIntro;
    public GameObject cutscene_RVSquatterOutro;
    public GameObject canvas_fadeIn;



    private void Awake()
    {
        EventData.gameStart = false;
        cutscene_overview.SetActive(false);
        cutscene_intro.SetActive(true);
        cutscene_bombLoserIntro.SetActive(false);
        cutscene_gateExplosion.SetActive(false);
        cutscene_hellHoundFight.SetActive(false);
        cutscene_RVSquatterIntro.SetActive(false);
        cutscene_RVSquatterOutro.SetActive(false);

    }
    void Start()
    {

        print("GameStart" + EventData.gameStart);
        GameManager.instance.FreezeBothPlayers();
        EventData.curSceneName = "Level_greyBox";

        EventData.isOnCutScene = true;
        EventData.craneIsActivated = false;
        cam_controlRoom.enabled = false;
        //dialogueSystemController.SetContinueMode(false);
        //dialogueSystemController.SetOriginalContinueMode();


    }

    // Update is called once per frame
    void Update()
    {
        ResetPlayerPos();
        CraneControl();
    }

    void ResetPlayerPos()
    {
        if(!isResetPos)
        {
            GameManager.Instance.ResetPlayersPosition(spawnPoints[0], spawnPoints[1]);
            isResetPos = true;
        }
    }

    #region Dialogue Function
    public void EnableDevilDialogue()
    {
        dialogueSystem_devil.enabled = true;
        print("Devil Dialogue Enabled");
    }

    public void DisableDevilDialogue()
    {
        dialogueSystem_devil.enabled = false;
        print("Devil Dialogue Disabled");
    }

    public void SwapBarkConversation(string conversation)
    {
        dialogueSystem_devil.barkConversation = conversation;
        print("Conversation swapped to: " + conversation);
    }
    #endregion

    #region Crane Movement & Control
    public void CraneControl()
    {
        if (!EventData.craneIsActivated) return;

        //1. access player input
        if (enterCrane.p1EnterCrane)
            playerInput = GameManager.instance.InputDetection_p1;


        else if (enterCrane.p2EnterCrane)
            playerInput = GameManager.instance.InputDetection_p2;

        print("Player Input Detected: " + playerInput.name);

        // reset player cam to crane cam 
        if (playerInput != null)
        {
            playerInput.playerCam = enterCrane.cam_crane;
        }
        //2. check for crouch input to exit crane
        if (playerInput.crouchPressed)
        {
            enterCrane.ResetEntrance();
        }


        //3. crane movement based on input
        float inputX = playerInput.GetHorizontalMovement_crane().x;
        float inputY = playerInput.GetHorizontalMovement_crane().z;
        //print("Crane Input X: " + inputX + " Y: " + inputY);
        if (Mathf.Abs(inputX) >= 0.5f && Mathf.Abs(inputX) > Mathf.Abs(inputY))
        {
            //move crane arm
            craneArm.Rotate(Vector3.up * inputX * craneRotateSpeed * Time.deltaTime);
        }

        if (Mathf.Abs(inputY) >= 0.5f && Mathf.Abs(inputY) > Mathf.Abs(inputX))
        {
            Vector3 delta = Vector3.up * inputY * craneMoveSpeed * Time.deltaTime * 0.1f;

            Vector3 localPos = craneArm.localPosition;

            localPos += delta;

            localPos.y = Mathf.Clamp(localPos.y, craneMinMaxHight.x, craneMinMaxHight.y);

            craneArm.localPosition = localPos;
            //move crane arm
            //craneArm.Translate(Vector3.up * inputY * craneMoveSpeed * Time.deltaTime);
        }



        float trolleyInputY = playerInput.GetHorizontalMovement_trolley().z;
        //float trolleyInputX = playerInput.GetHorizontalMovement_trolley().x;
        //if(Mathf.Abs(trolleyInputX) >= 0.5f)
        //{
        //    //move crane trolley
        //    craneTrolley.Translate(Vector3.right * trolleyInputX * craneMoveSpeed * Time.deltaTime);
        //}
        if (Mathf.Abs(trolleyInputY) >= 0.5f)
        {
            Vector3 localPos = craneTrolley.localPosition;

            localPos.z += trolleyInputY * craneMoveSpeed * Time.deltaTime;

            localPos.z = Mathf.Clamp(localPos.z, magnetMinMaxDistance.x, magnetMinMaxDistance.y);       
            
            craneTrolley.localPosition = localPos;

            //craneTrolley.Translate(Vector3.forward * trolleyInputY * craneMoveSpeed * Time.deltaTime,Space.Self);
            //Vector3 afterPos = craneTrolley.position + Vector3.right * trolleyInputY * craneMoveSpeed * Time.deltaTime;
            //craneTrolley.position = afterPos;
        }

        if (playerInput.attackPressed)
        {
            print("Attack Pressed - Enable Magnetic Surface");
            //enable magnetic surface
            EnableCraneMagneticSurface(inputX, magneticForce);
        }
        else
        {
            ReleaseMagnet();
        }
    }
    public void EnableCraneMagneticSurface(float inputXValue, float moveSpeed)
    {
        print("Enable Magnetic Surface - Check for Objects");
        //check for colliders within the magnetic surface area when enabled
        surfaceSize = new Vector3(surfaceSizeY, craneSurface.localScale.y / 2, craneSurface.localScale.z / 2) - offset;
        print("Surface Size: " + surfaceSize);
        surfaceCenter = craneSurface.localPosition;
        print("Surface Center: " + surfaceCenter);
        Collider[] colliders = Physics.OverlapBox(craneSurface.position, surfaceSize, craneSurface.rotation,
                                        1 << LayerMask.NameToLayer("Magnetic"), QueryTriggerInteraction.UseGlobal);
        print("Colliders Length: " + colliders.Length);
        if (colliders.Length > 0 && magnetCoroutine == null)
        {
            magneticSurfaceCollider = colliders[0];
            magneticObj = magneticSurfaceCollider.transform;


            magnetCoroutine = StartCoroutine(MagnetPullCoroutine(magneticObj));
        }
        if (colliders.Length > 0)
        {
            magneticSurfaceCollider = colliders[0];
            Transform magneticObj = colliders[0].transform;

            Rigidbody rb = magneticSurfaceCollider.attachedRigidbody;
            if (rb == null) return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;

            Vector3 targetPos = craneSurface.position + craneSurface.up * 0.2f;
            Quaternion targetRot = craneSurface.rotation;

            magneticObj.position = Vector3.Lerp(magneticObj.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(magneticObj.position, targetPos) < 0.2f)
            {
                magneticObj.SetParent(craneTrolley, true);
                childObjs = magneticObj;
            }

        }
        else
        {
            if (magneticSurfaceCollider == null) return;
            Rigidbody rb = magneticSurfaceCollider.attachedRigidbody;
            rb.isKinematic = false;
            rb.useGravity = true;
            if (childObjs != null)
            {
                childObjs.SetParent(null, true);
                childObjs = null;
            }
            magneticSurfaceCollider = null;
        }
        //apply the force to the first object detected within the magnetic surface area
        if (colliders.Length > 0)
        {
            Rigidbody rigi_mo = colliders[0].gameObject.GetComponent<Rigidbody>();
            rigi_mo.linearVelocity = Vector3.up * magneticForce;
            Transform magneticObj = colliders[0].transform;
            Vector3 afterPos = magneticObj.position + Vector3.right * inputXValue * moveSpeed * Time.deltaTime;
            magneticObj.position = afterPos;
            //switch (craneState)
            //{
            //    case CraneStateType.ActivateOneObject:
            //        Rigidbody rigi_mo = colliders[0].gameObject.GetComponent<Rigidbody>();
            //        rigi_mo.linearVelocity = Vector3.up * magneticForce;
            //        Transform magneticObj = colliders[0].transform;
            //        Vector3 afterPos = magneticObj.position + Vector3.right * inputXValue * moveSpeed * Time.deltaTime;
            //        magneticObj.position = afterPos;
            //        //Transform magneticObj = colliders[0].transform;
            //        //if(magneticObj.parent != craneTrolley)
            //        //{
            //        //    magneticObj.transform.SetParent(craneTrolley);
            //        //    childObjs = magneticObj;

            //        //}

            //        break;
            //    case CraneStateType.ActivateMultipleObjects:
            //        //for (int i = 0; i < colliders.Length; i++)
            //        //{
            //        //    magneticObj = colliders[i].transform;
            //        //    if(magneticObj.parent != craneTrolley)
            //        //    {
            //        //        magneticObj.transform.SetParent(craneTrolley);
            //        //        childObjs = magneticObj;
            //        //    }


            //        //}
            //        break;
            //}
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        //Visualize the magnetic surface area in the editor
        if (craneSurface == null) return;

        Gizmos.color = Color.yellow;  // Choose any color you like
        Gizmos.matrix = Matrix4x4.TRS(
            craneSurface.position,
            craneSurface.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(surfaceCenter, surfaceSize * 2);
    }
    /***************************/
    IEnumerator MagnetPullCoroutine(Transform obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) yield break;

        // Disable physics while pulling
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        while (true)
        {
            // Attach when close enough
            print("distance to crane surface: " + Vector3.Distance(obj.position, craneSurface.position));
            if (Vector3.Distance(obj.position, craneSurface.position) < 1.7f)
            {
                magneticForce = 0;
                obj.SetParent(craneTrolley, true); // follows craneArm automatically
                childObjs = obj;
                break;
            }
            else
            {
                magneticForce = 5f;
                // Target follows craneSurface every frame
                Vector3 targetPos = craneSurface.position + craneSurface.up * 0.1f; ;

                if (targetPos.y < craneMinMaxHight.y - 0.5f)
                {
                    targetPos.y = craneMinMaxHight.y - 0.5f;
                }

                obj.position = Vector3.Lerp(obj.position, targetPos, magneticForce * Time.deltaTime);
                obj.rotation = craneTrolley.rotation;
            }

            yield return null;
        }

        magnetCoroutine = null;
    }
    /****************************/
    public void ReleaseMagnet()
    {
        if (magnetCoroutine != null)
        {
            StopCoroutine(magnetCoroutine);
            magnetCoroutine = null;
        }

        if (childObjs != null)
        {
            Rigidbody rb = childObjs.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }

            childObjs.SetParent(null, true);
            childObjs = null;
        }

        if (magneticObj != null)
        {
            Rigidbody rb = magneticObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }
            magneticObj = null;
        }
    }

    #region Cutscene Control
    IEnumerator OnCutsceneWait(float duration)
    {
        print("Cutscene Wait Started");
        yield return new WaitForSeconds(duration);
        print("Cutscene Wait Ended");
        EventData.isOnCutScene = false;
        OnGameStart();
    }
    #endregion

    public void OnGameStart()
    {
        StartCoroutine(StartTimer(1f));
        EventData.isOnCutScene = false;
        GameManager.instance.ResetPlayer1Position(spawnPoints[2]);
        GameManager.instance.ResetPlayer2Position(spawnPoints[3]);
        GameManager.instance.UnFreezeBothPlayers();
        EventData.gameStart = true;
        print("GameStart" + EventData.gameStart);
    }

    IEnumerator StartTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        canvas_fadeIn.SetActive(false);
    }
}
