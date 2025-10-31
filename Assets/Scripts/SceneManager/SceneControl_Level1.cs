using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneControl_Level1 : SceneControlBase<SceneControl_Level1>
{
    public Transform[] enterPoints;
    public Transform exitCranePoint;
    public EnterCrane enterCrane;

    public Transform crane;
    private PlayerInputDetection playerInput;

    [Header("Crane Surface Movement Variables")]
    //public float rotationSpeed = 30;
    public float moveSpeed = 2;

    [Header("Testing Scene")]
    public GameObject playerObj;
    public PlayerStateMachine playerStateMachine;
    public PlayerInputDetection playerInputDetection;
    public float inputXValue;
    public GameObject craneMagneticSurface;
    public float surfaceSizeY;
    public Vector3 surfaceSize;
    public Vector3 surfaceCenter;
    public float magneticForce;

    private void Awake()
    {
        GameManager.instance.craneObject = crane.gameObject;
        print("CraneObject = " + GameManager.instance.craneObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ResetPlayerPos();
        EventData.curSceneName = "Level1";
        EventData.craneIsActivated = false;

    }

    // Update is called once per frame
    void Update()
    {
        EnterCraneScriptControl();
        ResetPlayerPos();
    }

    public void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            //GameManager.instance.player1.transform.position = enterPoints[0].position;
            //GameManager.instance.player2.transform.position = enterPoints[1].position;
            isResetPos = true;
        }
    }

    public void EnterCraneScriptControl()
    {
        if (EventData.craneIsActivated && enterCrane.GetComponent<Collider>().enabled)
        {
            enterCrane.GetComponent<Collider>().enabled = false;
        }
        else if (!EventData.craneIsActivated && !enterCrane.GetComponent<Collider>().enabled)
        {
            enterCrane.GetComponent<Collider>().enabled = true;
        }

        CraneMovement();
    }

    public void CraneMovement()
    {
        //player are able to control crane by using horizontal movement and exit crane by pressing attack button
        if (enterCrane.p1EnterCrane)
        {
            playerStateMachine.FreezeStateMachine();
            print("Player 1 in Crane and Freeze");

            //GameManager.instance.FreezePlayer1();
            //playerInput = GameManager.instance.player1.GetComponent<PlayerInputDetection>();

            // Calculate rotation based on horizontal input
            inputXValue = playerInputDetection.GetHorizontalMovement().x;
            //float inputY = playerInputDetection.horizontalInputValue.y;
            //print(inputY);
            //crane.Rotate(Vector3.up, inputX * rotationSpeed * Time.deltaTime);
            if(Mathf.Abs(inputXValue) >= 0.02)
            {
                Transform surfacePos = craneMagneticSurface.transform;
                Vector3 afterPos = surfacePos.position + Vector3.right * inputXValue * moveSpeed * Time.deltaTime;

                surfacePos.position = afterPos;

            }


            if (playerInputDetection.attackPressed)
            {
                enterCrane.p1EnterCrane = false;
                EventData.craneIsActivated = false;
                //GameManager.instance.UnFreezePlayer1();
                playerStateMachine.UnFreezeStateMachine();
                playerInputDetection.transform.position = exitCranePoint.position;
                return;
            }

            // Enable magnetic surface when crouch is pressed
            if (playerInputDetection.crouchPressed)
            {
                print("player 1 enable magnetic surface");
                EnableCraneMagneticSurface();
            }
        }

        if (enterCrane.p2EnterCrane)
        {
            GameManager.instance.FreezePlayer2();
            playerInput = GameManager.instance.player2.GetComponent<PlayerInputDetection>();
            // Calculate rotation based on horizontal input
            float inputX = playerInput.GetHorizontalMovement().x;
            //crane.Rotate(Vector3.up, inputX * rotationSpeed * Time.deltaTime);
            if (playerInput.attackPressed)
            {
                enterCrane.p2EnterCrane = false;
                EventData.craneIsActivated = false;
                GameManager.instance.UnFreezePlayer2();
                playerInput.transform.position = exitCranePoint.position;
                return;
            }
        }

    }

    public void EnableCraneMagneticSurface()
    {
        //check for colliders within the magnetic surface area when enabled
        surfaceSize = new Vector3(craneMagneticSurface.transform.localScale.x/2, surfaceSizeY, craneMagneticSurface.transform.localScale.z/2);
        surfaceCenter = craneMagneticSurface.transform.position + -Vector3.up * surfaceSizeY;
        Collider[] colliders = Physics.OverlapBox(surfaceCenter, surfaceSize,craneMagneticSurface.transform.rotation,
                                        1 << LayerMask.NameToLayer("Magnetic"), QueryTriggerInteraction.UseGlobal);
        print("Colliders Length: " + colliders.Length);

        //apply the force to the first object detected within the magnetic surface area
        if (colliders.Length > 0 )
        {
            //colliders[0].gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * magneticForce, ForceMode.Impulse);
            //print(colliders[0].name + "is moving up");
            //Transform magObjPos = colliders[0].transform;
            //magObjPos.Translate(playerInputDetection.GetHorizontalMovement());
            Rigidbody rigi_mo = colliders[0].gameObject.GetComponent<Rigidbody>();
            rigi_mo.linearVelocity = Vector3.up * magneticForce;
            Transform magneticObj = colliders[0].transform;
            Vector3 afterPos = magneticObj.position + Vector3.right * inputXValue * moveSpeed * Time.deltaTime;
            magneticObj.position = afterPos;
        }


    }

    private void OnDrawGizmos()
    {
        //Visualize the magnetic surface area in the editor
        if (craneMagneticSurface == null) return;

        Gizmos.color = Color.cyan;  // Choose any color you like
        Gizmos.matrix = Matrix4x4.TRS(
            craneMagneticSurface.transform.position,
            craneMagneticSurface.transform.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(Vector3.zero + -Vector3.up * surfaceSizeY, surfaceSize * 2);

    }
}


