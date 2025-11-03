using UnityEngine;

public enum CraneStateType
{
    ActivateOneObject,
    ActivateMultipleObjects,
    
}
public class CraneMovement : MonoBehaviour
{
    public Transform exitCranePoint;
    public EnterCrane enterCrane;

    [Header("Crane Surface Movement Variables")]
    public float moveSpeed = 2;
    public GameObject craneMagneticSurface;
    public float surfaceSizeY;
    private Vector3 surfaceSize;
    private Vector3 surfaceCenter;
    public float magneticForce;
    public float inputXValue;

    public GameObject visualSurface;
    public Vector3 offset;
    public CraneStateType craneState = CraneStateType.ActivateOneObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CraneMovementControl();
    }

    public void ExitCrane(Transform player)
    {
        player.position = exitCranePoint.position;
        enterCrane.ResetEntrance();

    }

    public void CraneMovementControl()
    {
        if(EventData.craneIsActivated == false) visualSurface.SetActive(false);

        if (enterCrane.p1EnterCrane)
        {
            // Get Player 1 Input
            PlayerInputDetection playerInput = GameManager.instance.InputDetection_p1;
            inputXValue = playerInput.GetHorizontalMovement().x;

            // Move the crane along the magnetic surface
            if (Mathf.Abs(inputXValue) >= 0.02)
            {
                Transform surfacePos = craneMagneticSurface.transform;
                Vector3 afterPos = surfacePos.position + Vector3.right * inputXValue * moveSpeed * Time.deltaTime;

                surfacePos.position = afterPos;
            }

            // Exit crane if attack button is pressed
            if (playerInput.attackPressed)
            {
                enterCrane.p1EnterCrane = false;
                EventData.craneIsActivated = false;
                Transform playerPos = GameManager.instance.player1.transform;
                ExitCrane(playerPos);
                return;
            }

            // Enable magnetic surface when crouch is pressed
            if(playerInput.crouchPressed)
            {
                print("Enable Magnetic Surface");
                EnableCraneMagneticSurface();
                visualSurface.SetActive(false);
            }
            else
                visualSurface.SetActive(true);

        }

        if (enterCrane.p2EnterCrane)
        {
            // Get Player 2 Input
            PlayerInputDetection playerInput = GameManager.instance.InputDetection_p2;
            inputXValue = playerInput.GetHorizontalMovement().x;

            // Move the crane along the magnetic surface
            if (Mathf.Abs(inputXValue) >= 0.02)
            {
                Transform surfacePos = craneMagneticSurface.transform;
                Vector3 afterPos = surfacePos.position + Vector3.right * inputXValue * moveSpeed * Time.deltaTime;

                surfacePos.position = afterPos;
            }

            // Exit crane if attack button is pressed
            if (playerInput.attackPressed)
            {
                enterCrane.p2EnterCrane = false;
                EventData.craneIsActivated = false;
                Transform playerPos = GameManager.instance.player2.transform;
                ExitCrane(playerPos);
                return;
            }

            // Enable magnetic surface when crouch is pressed
            if (playerInput.crouchPressed)
            {
                print("Enable Magnetic Surface");
                EnableCraneMagneticSurface();
                visualSurface.SetActive(false);
            }
            else
                visualSurface.SetActive(true);

        }
    }

    public void EnableCraneMagneticSurface()
    {
        //check for colliders within the magnetic surface area when enabled
        surfaceSize = new Vector3(craneMagneticSurface.transform.localScale.x / 2, surfaceSizeY, craneMagneticSurface.transform.localScale.z / 2) - offset;
        surfaceCenter = craneMagneticSurface.transform.position + -Vector3.up * surfaceSizeY;
        Collider[] colliders = Physics.OverlapBox(surfaceCenter, surfaceSize, craneMagneticSurface.transform.rotation,
                                        1 << LayerMask.NameToLayer("Magnetic"), QueryTriggerInteraction.UseGlobal);
        print("Colliders Length: " + colliders.Length);

        //apply the force to the first object detected within the magnetic surface area
        if (colliders.Length > 0)
        {
            switch(craneState)
            {
                case CraneStateType.ActivateOneObject:
                    Rigidbody rigi_mo = colliders[0].gameObject.GetComponent<Rigidbody>();
                    rigi_mo.linearVelocity = Vector3.up * magneticForce;
                    Transform magneticObj = colliders[0].transform;
                    Vector3 afterPos = magneticObj.position + Vector3.right * inputXValue * moveSpeed * Time.deltaTime;
                    magneticObj.position = afterPos;
                    break;
                case CraneStateType.ActivateMultipleObjects:
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        rigi_mo = colliders[i].gameObject.GetComponent<Rigidbody>();
                        rigi_mo.linearVelocity = Vector3.up * magneticForce;
                        magneticObj = colliders[i].transform;
                        afterPos = magneticObj.position + Vector3.right * inputXValue * moveSpeed * Time.deltaTime;
                        magneticObj.position = afterPos;
                    }
                    break;
            }
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
