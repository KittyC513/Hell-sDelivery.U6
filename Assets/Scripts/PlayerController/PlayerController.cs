//Comment out `undef` or `def Newwork` to switch between script versions
// need to set on CameraMovement_Player.cs as well
#define Network
#undef Network

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.Netcode;

//this script acts as our player controller blackboard, all our variables that states will need to access are in here
//as well as any functionality that will always need to be active regardless of state such as basic movement, ground checks and rotation
//there can also be toggles to disable this functionality but in general most states will need to use it
public class PlayerController : NetworkBehaviour
{
    [Header ("Basic Movement Variables")]
    [SerializeField] private float maxRunSpeed = 10; //the max speed the player can run
    [SerializeField] private float acceleration = 2; //how fast speed is added to the player when moving
    [SerializeField] private float decceleration = 2; //how fast the player slows down when no longer inputting
    [SerializeField] private float maxAccelStep = 150; //the maximum value that the player's velocity can be moved by in a single frame
    [SerializeField] private float rotationSpeed = 500; //how fast the player rotates


    Vector3 goalVelocityChange; //used to determine how much velocity we need to change to reach our desired velocity
    private Vector3 leftStickDir;

    private bool frozen = false;


    [Header("Ground Check Variables")]
    [SerializeField] private LayerMask groundLayers; //what layers are ground
    private bool grounded = false;
    private float groundAngle = 0; //the angle of the ground we are walking on
    private float groundCheckFactor = 1; //how much our raycast distance is multiplied by, this is used for sticking to slopes better might find a better solution in the future
    [SerializeField] private bool visualizeRaycast = false;
    private Vector3 raycastStartPoint; //where the ground check raycast starts

    [Space, Header("Land Spring Variables")]
    [SerializeField] private float floatHeight = 0.2f; //how far the player hitbox floats above the ground
    [SerializeField] private float raycastDist = 0.35f; //the max distance for ground checking
    [SerializeField] private float raycastRadius = 0.23f;
    [SerializeField] private float floatHeightStrength = 100; //how fast the player readjusts the y position when resetting to float
    [SerializeField] private float floatHeightDamping = 4f; //used to determine how much the player springs when landing

    [Space, Header("Air Velocity Variables")]
    [SerializeField] private float maxFallSpeed = 10; //the maximum speed the player can fall
    [SerializeField] private float fallAccel = 25; //how fast the player gains speed when falling downwards
    [SerializeField] private float maxFallAccelStep = 150;

    [Space, Header("Jump Variables")]
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float jumpDecayRate = 40; //basically gravity when jumping or how fast the upwards force goes down
    [SerializeField] private float jumpReleaseFactor = 2f; //how much gravity is increased when the jump button is let go of
    [SerializeField] private float jumpPeakFactor = 0.3f; //how much gravity is changed at the peak of the jump to give longer air time
    [SerializeField] private float jumpPeakRange = 1.2f; //what velocity value will slow down velocity at the peak of a jump

    [Space, Header ("Double Jump Variables")]
    [SerializeField] private float doubleJumpHeight = 1f; //jump height when double jumping
    [SerializeField] private float doubleJumpDecayRate = 35; //how fast the double jump decays (essentially gravity pulling down on the upwards jump force)
    [SerializeField] private int maxJumps = 2; //maximum jumps including grounded jump
    [HideInInspector] public float remainingJumps; //how many jumps the player currently has 

    [Space, Header("Sliding Variables")]
    [SerializeField] private float maxSlideSpeed = 14; //how fast the player can slide at maximum speed
    [SerializeField] private float slideAccel = 50; //how fast player builds up speed while sliding
    [SerializeField] private float maxSlopeAngle = 45; //the maximum slope our player can walk on
    [SerializeField] private float maxSlideAccelStep = 200; 
   

    [Space, Header("Ledge Grab Variables")]
    [SerializeField] private float ledgeGrabHorizontalRange = 0.6f; //how far out the ledge grab detection ray is
    [SerializeField] private float ledgeGrabUpwardsRange = 0.44f; //how far up the ledge raycast starts
    [SerializeField] private float ledgeGrabDownwardsRange = 0.37f; //how far down the ledge raycast shoots 

    [SerializeField] private float xzHangOffset = 0.5f; //how far away from the ledge on the xz axis the player is while ledge hanging
    [SerializeField] private float yHangOffset = 0.3f; //how far away from the ledge on the y axis the player is while ledge hanging

    [SerializeField] private LayerMask ledgeGrabMask; //what layermasks can be ledge grabbed (usually same as ground)
    

    private Vector3 lastLedgeXZ; //the location of the last ledge detection on the X and Z plane
    private float lastLedgeY; //the location of the last ledge detecting on the Y plane

   

    [Space, Header("Quality Of Life Variables")]
    [SerializeField] private float coyoteTime = 0.1f; //how long after running off a ledge can the player still input jump
    private float coyoteCurrent = 0;
    private bool canCoyote = false;
    private bool freezeRotation = false;

    [SerializeField] private AnimationCurve quickTurnMultiplier; //a curve that determines how much velocity gain is multiplied by when turning sharply, makes for a quicker turn around
    private float playerHitboxHeight; //the value of the players hitbox with scale transforms applied

    [Space, Header("References")]
    [SerializeField] private PlayerInputDetection inputDetection;
    private Rigidbody rb;
    [SerializeField] private PlayerAttackControl aControl;

    [Space, Header("Debug")]
    [SerializeField] private float currentSpeed;

    [Space, Header("Lock")]
    public CameraMovement_Lock lockCam;
    public Transform lockTarget;

    //these variables are all accessable to the various states

    //General Variables
    public Rigidbody RB { get { return rb; }}
    public bool Grounded { get { return grounded; } }
    public float PlayerHitboxHeight { get { return playerHitboxHeight; } }

    //Grounded Variables
    public float FloatHeight { get { return floatHeight; } }
    public float FloatHeightStrength { get { return floatHeightStrength; } }
    public float FloatHeightDamping { get { return floatHeightDamping; } }
    public float GroundAngle { get { return groundAngle; } }
    public float MaxSlopeAngle { get { return maxSlopeAngle; } }

    //Falling Variables
    public float MaxFallSpeed { get { return maxFallSpeed; }}
    public float FallAccel { get { return fallAccel; }}
    public float MaxFallAccelStep { get { return maxFallAccelStep; }}

    //Jumping Variables
    public float JumpHeight { get { return jumpHeight; }}
    public float JumpDecayRate { get { return jumpDecayRate; }}
    public float DoubleJumpHeight { get { return doubleJumpHeight; }}
    public float DoubleJumpDecayRate { get { return doubleJumpDecayRate; }}
    public float MaxJumps { get { return maxJumps; }}
    public float JumpReleaseFactor { get { return jumpReleaseFactor; }}
    public float JumpPeakFactor { get { return jumpPeakFactor; }}
    public float JumpPeakRange {  get { return jumpPeakRange; }}

    //Sliding Variables
    public float MaxSlideSpeed {  get { return maxSlideSpeed; }}
    public float SlideAccel {  get { return slideAccel; }}
    public float MaxSlideAccelStep {  get { return maxSlideAccelStep; }}

    //Ledge Grab Variables
    public float LedgeGrabHorizontalRange { get { return ledgeGrabHorizontalRange; }}
    public float LedgeGrabUpwardsRange { get  { return ledgeGrabUpwardsRange; }}
    public float LedgeGrabDownwardsRange { get { return ledgeGrabDownwardsRange; }}
    public LayerMask LedgeGrabMask { get { return ledgeGrabMask; }}
    public float XZHangOffset { get { return xzHangOffset; }}
    public float YHangOffset { get { return yHangOffset; }}

    public Vector3 LastLedgeXZ { get { return lastLedgeXZ; }}
    public float LastLedgeY {  get { return lastLedgeY; }}


    //QOL Variables
    public bool CanCoyote { get { return canCoyote; }}

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        float unscaledHeight = GetComponent<CapsuleCollider>().height;
        playerHitboxHeight = unscaledHeight * transform.localScale.y;
    }

    private void Start()
    {

    }

    private void Update()
    {
#if Network
        if(!IsOwner) return;
#endif

        ReadInputs(); //reads movement inputs
        DetectGround(); //detect ground and slopes
        CoyoteTime(); //determines if coyote time is active
  
    }

    private void FixedUpdate()
    {

        if (!frozen)
        {
                CalculateMovement(rb, leftStickDir, acceleration, decceleration, maxRunSpeed);
        }

    }

    //get our jump inputs from the player input script
    public bool DetectJumpInput() //a single jump press (does not detect holding down the button)
    {
        
        return inputDetection.JumpBuffered();
    }

    public bool DetectJumpHold() //returns true while the jump button is held
    {
       return inputDetection.jumpPressed;
    }

    public Vector3 ReadInputs()
    {
        //get the left stick inputs from the player input script
        return leftStickDir = inputDetection.GetHorizontalMovement();
    }

    public bool DetectCrouchInput()
    {
        return inputDetection.crouchPressed;
    }

    //this function needs to run regardless of if the player is grounded or airborne, this is the basic movement
    //therefore it needs to be in the main script
    private void CalculateMovement(Rigidbody rb, Vector3 dir, float accelValue, float decelValue, float maxSpeed)
    {
        Vector3 currentVel = rb.linearVelocity;

        Vector3 targetDir = dir;

        
        //this is the speed we are trying to reach / our maximum speed with a direction provided by a camera dependant input
        Vector3 targetVelocity = targetDir * (maxSpeed);

        //our current desired velocity direction
        Vector3 unitVel = goalVelocityChange.normalized;

        //the difference between our new target direction and our current target direction
        float velDot = Vector3.Dot(targetDir, unitVel);

        //Debug.Log(unitVel + "a" + targetDir);

        //the magnitude of only the x and z values
        float xzMagnitude = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        currentSpeed = xzMagnitude;

        //affect acceleration based on the difference in our directions this lets us turn around quickly if we input a complete opposite direction 
        float accel = quickTurnMultiplier.Evaluate(velDot) * accelValue;
        float decel = decelValue;

        //if the target velocity is going towards 0 or the player is no longer inputting we use a decceleration value to have control over accel and deccel seperately
        if (targetVelocity.magnitude <= 0.05f)
        {
            //how much we will change our velocity next step with smoothing by vector3.movetowards
            //0.02 is unity's default fixedupdate timestep, i use this value right now because i dont know how to reference that variable
            goalVelocityChange = Vector3.MoveTowards(goalVelocityChange, targetVelocity, decel * 0.02f);
            
            //the amount of velocity change needed to reach our maximum velocity
            Vector3 velocityChange = (goalVelocityChange - currentVel) / 0.02f;

            //maxAccelStep limits how much our velocity can change per step
            velocityChange = Vector3.ClampMagnitude(velocityChange, maxAccelStep);
            velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
            //Debug.Log(velocityChange * rb.mass);
            //apply our force to our velocity
            

            rb.AddForce(velocityChange * rb.mass);
            
        }
        else
        {
            //how much we will change our velocity next step with smoothing by vector3.movetowards
            goalVelocityChange = Vector3.MoveTowards(goalVelocityChange, targetVelocity, accel * 0.02f);

            //the amount of velocity change needed to reach our maximum velocity
            Vector3 velocityChange = (goalVelocityChange - currentVel) / 0.02f;

            //maxAccelStep limits how much our velocity can change per step
            velocityChange = Vector3.ClampMagnitude(velocityChange, maxAccelStep);
           
            //apply our force to our velocity
            velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

            

            rb.AddForce(velocityChange * rb.mass);
        }
        
        if (!freezeRotation) RotateTowards(targetDir, rotationSpeed);

    }

    private void RotateTowards(Vector3 direction, float rotationSpeed)
    {
        //get our desired direction ignoring y 
        direction = new Vector3(direction.x, 0, direction.z);

        /********************************************************************************/
        if (lockCam.isLockTrigger)
        {
            if (direction.magnitude > 0)
                this.transform.LookAt(lockTarget);
        }
        /********************************************************************************/
        else
        {
            if (direction.magnitude > 0)
            {
                //calculate our desired rotation
                Quaternion toRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

                //use rotate towards to rotate to our desired position by our rotation speed rather than all at once
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, (rotationSpeed) * Time.fixedDeltaTime);
            }
        }
     
    }

   

    public RaycastHit DetectGround()
    {
        RaycastHit hit;
        raycastStartPoint = new Vector3(transform.position.x, (transform.position.y + 0.1f + raycastRadius) - (playerHitboxHeight / 2), transform.position.z);
        //Cast a ray downwards to detect any objects with the ground layermask
        if (Physics.SphereCast(raycastStartPoint, raycastRadius, -transform.up, out hit, raycastDist * groundCheckFactor, groundLayers))
        {
            //Direction towards the raycast hit point from the raycast origin
            Vector3 directionToHit = (hit.point - raycastStartPoint).normalized;

            float sphereCastAngle = Vector3.Angle(hit.normal, Vector3.up);

            //only cast the second raycast if there is any slope we need to double check
            //this takes out unneccesary raycasts
            if (sphereCastAngle > 0)
            {
                //this raycast is used because spherecast is returning a weird angle by detecting the corners of cubes rather than the surface
                //this raycast will be cast from the same point at the same hit point but wont return a weird angle value from the corner
                Physics.Raycast(raycastStartPoint, directionToHit, out RaycastHit test, Mathf.Infinity, groundLayers);

                groundAngle = Vector3.Angle(test.normal, Vector3.up);
            }
            else
            {
                groundAngle = 0;
            }

            //if we are grounded extend the raycast distance so that we don't fall off of the slope
            if (groundAngle > 10)
            {
                groundCheckFactor = 2.5f;
            }
            else if (groundAngle < 25 || groundAngle > maxSlopeAngle)
            {
                groundCheckFactor = 1;
            }
            
            grounded = true;
            

            return hit;
        }
        else
        {
            groundCheckFactor = 1; 
            groundAngle = 0;
            grounded = false;
            return hit;
        }
    }

    public Vector3 SlopeSlideVelocity(RaycastHit hit)
    {
        return Vector3.ProjectOnPlane(Vector3.down, hit.normal);
    }

    //this function currently does not work and im not sure why
    public void LimitPlayerVelocity(float velocityCap, string scriptName)
    {
        //this function can be used to limit the player's current velocity
        //try not to limit it elsewhere so we know what is affected the rigidbody if anything goes wrong
        //this function should be run in update or fixed update

        Debug.Log("Velocity is being limited to " + velocityCap + " by " + scriptName);
        //maxRunSpeed = Mathf.Clamp(maxRunSpeed, -velocityCap, velocityCap);
    }

    public void ResetCoyoteTime()
    {
        coyoteCurrent = 0;
    }

    private void CoyoteTime()
    {
        if (coyoteCurrent < coyoteTime)
        {
            coyoteCurrent += Time.deltaTime;
        }

        if (coyoteCurrent <= coyoteTime)
        {
            canCoyote = true;
        }
        else
        {
            canCoyote = false;
        }
    }


    private void OnDrawGizmos()
    {
        if (visualizeRaycast)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(raycastStartPoint, DetectGround().point);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(raycastStartPoint, raycastRadius);

            Gizmos.color = Color.green;

            Vector3 topForwardPos = transform.position + (new Vector3(0, ledgeGrabUpwardsRange, 0));
            Vector3 bottomRayPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            Vector3 downRayPos = transform.position + (new Vector3(ledgeGrabHorizontalRange * transform.TransformDirection(Vector3.forward).x, ledgeGrabUpwardsRange, ledgeGrabHorizontalRange * transform.TransformDirection(Vector3.forward).z));
            Vector3 direction = transform.TransformDirection(Vector3.forward);

            Gizmos.DrawLine(downRayPos, new Vector3(downRayPos.x, downRayPos.y - ledgeGrabDownwardsRange, downRayPos.z));
            Gizmos.DrawRay(topForwardPos, direction);
        }
    }

    public void SetFreezeState(bool state, string scriptName)
    {
        frozen = state;
        if (state == true)
        {
            rb.linearVelocity = Vector3.zero;
            goalVelocityChange = Vector3.zero;
        }
        Debug.Log("Player controller freeze state set to " + state + " by " + scriptName);
    }

    public void FreezeRotation(bool freezeState, string scriptName)
    {
        freezeRotation = freezeState;
        Debug.Log("Player freeze rotation state set to " + freezeState + " by " + scriptName);
    }

    public void SetLedgeSnapVariables(Vector3 xz, float y)
    {
        lastLedgeXZ = xz;
        lastLedgeY = y;
    }

    public bool CheckCanAttack()
    {
        return aControl.canAttack;
    }



}
