using UnityEngine;
using UnityEngine.InputSystem;

public class SceneControl_CraneTestinhg : SceneControlBase<SceneControl_CraneTestinhg>
{
    public Transform[] spawnpoints;

    [Header("Crane Control")]
    public Transform craneArm;
    public Transform craneTrolley;
    public EnterCrane enterCrane;
    PlayerInputDetection playerInput;
    public float craneMoveSpeed = 5f;
    public float craneRotateSpeed = 20f;

    public float surfaceSizeY;
    private Vector3 surfaceSize;
    private Vector3 surfaceCenter;
    public Vector3 offset;
    public CraneStateType craneState = CraneStateType.ActivateOneObject;
    public Transform childObjs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "CraneTesting";
        EventData.craneIsActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        ResetPlayerPos();
        CraneControl();
    }

    public void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            GameManager.instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
            isResetPos = true;
        }
    }

    #region Crane Control
    public void CraneControl()
    {
        if (!EventData.craneIsActivated) return;

        //1. access player input
        if (enterCrane.p1EnterCrane)
            playerInput = GameManager.instance.InputDetection_p1;

        else if(enterCrane.p2EnterCrane)
            playerInput = GameManager.instance.InputDetection_p2;

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
        print("Crane Input X: " + inputX + " Y: " + inputY);
        if (Mathf.Abs(inputX) >= 0.5f && Mathf.Abs(inputX) > Mathf.Abs(inputY))
        {
            //move crane arm
            craneArm.Rotate(Vector3.up * inputX * craneRotateSpeed * Time.deltaTime);
        }
        
        if(Mathf.Abs(inputY) >= 0.5f && Mathf.Abs(inputY) > Mathf.Abs(inputX))
        {
            //move crane arm
            craneArm.Translate(Vector3.up * inputY * craneMoveSpeed * Time.deltaTime);
        }

        if(playerInput.attackPressed)
        {
            //enable magnetic surface
            EnableCraneMagneticSurface();
        }
        else
        {
            craneTrolley.DetachChildren();
        }
    }

    public void EnableCraneMagneticSurface()
    {
        //check for colliders within the magnetic surface area when enabled
        surfaceSize = new Vector3(craneTrolley.localScale.x / 2, surfaceSizeY, craneTrolley.localScale.z / 2) - offset;
        surfaceCenter = craneTrolley.position + -Vector3.up * surfaceSizeY;
        Collider[] colliders = Physics.OverlapBox(surfaceCenter, surfaceSize, craneTrolley.rotation,
                                        1 << LayerMask.NameToLayer("Magnetic"), QueryTriggerInteraction.UseGlobal);
        print("Colliders Length: " + colliders.Length);

        //apply the force to the first object detected within the magnetic surface area
        if (colliders.Length > 0)
        {
            switch (craneState)
            {
                case CraneStateType.ActivateOneObject:
                    Transform magneticObj = colliders[0].transform;
                    if(magneticObj.parent != craneTrolley)
                    {
                        magneticObj.transform.SetParent(craneTrolley);
                        childObjs = magneticObj;

                    }

                    break;
                case CraneStateType.ActivateMultipleObjects:
                    //for (int i = 0; i < colliders.Length; i++)
                    //{
                    //    magneticObj = colliders[i].transform;
                    //    if(magneticObj.parent != craneTrolley)
                    //    {
                    //        magneticObj.transform.SetParent(craneTrolley);
                    //        childObjs = magneticObj;
                    //    }


                    //}
                    break;
            }
        }
    }
    #endregion
}
