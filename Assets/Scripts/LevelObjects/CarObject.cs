using System;
using System.Linq;
using UnityEngine;
public class CarObject : MonoBehaviour
{
    [SerializeField] private float targetHeight;
    [SerializeField] private float frequency;
    [SerializeField] private float damping;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector3 maxRotationAngles;
    private Rigidbody rb;

    [SerializeField] private Transform frontLTire;
    [SerializeField] private Transform backLTire;
    [SerializeField] private Transform frontRTire;
    [SerializeField] private Transform backRTire;

    [SerializeField] private float tireRadius = 0.5f;

    [SerializeField] private float maxDriveSpeed = 18f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxAccelStep = 55;
    [SerializeField] private bool driving = false;

    private bool grounded = false;

    public Quaternion rbRot;

    private Vector3 goalVelocityChange;
    private Vector3 startRot;
    private Ray tireRay;
    private Ray groundRay;
    private RaycastHit[] raycastHits;
    private RaycastHit[] groundHit;

    //how do i apply force in 4 spots for each tire???

   //imagine the object is standing on the 4 tires
   //each point is holding a portion of the car up

    private void Start()
    {
        tireRay = new Ray();
        groundRay = new Ray();
        rb = GetComponent<Rigidbody>();
        raycastHits = new RaycastHit[1];
        groundHit = new RaycastHit[1];
        startRot = transform.rotation.eulerAngles;
    }

    private void FixedUpdate()
    {
        //check for ground first then snap each tire to position
        DetectGrounded(transform.position, targetHeight + 1.05f, groundMask);

        if (grounded)
        {
           

            //snap to the ground at each tire position
            DetectTireGround(frontLTire.position, targetHeight + 1f, groundMask, frontLTire.GetChild(0).gameObject);
            DetectTireGround(frontRTire.position, targetHeight + 1f, groundMask, frontRTire.GetChild(0).gameObject);
            DetectTireGround(backLTire.position, targetHeight + 1f, groundMask, backLTire.GetChild(0).gameObject);
            DetectTireGround(backRTire.position, targetHeight + 1f, groundMask, backRTire.GetChild(0).gameObject);
        }
        else
        {
             rb.AddForce(Vector3.down * (65 * rb.mass));
        }

        //clamp the velocity to stop the car gaining any massive forces from weird physics interactions
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, frequency*4);

        //freeze the cars y rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, startRot.y, transform.rotation.eulerAngles.z);

        if (driving)
        {
            rb.constraints = RigidbodyConstraints.None;
            CalculateMovement(rb, transform.forward, acceleration, maxDriveSpeed, maxAccelStep);
            tag = "LevelObject";
        }
    }

    public void TriggerDriving()
    {
        driving = true;
    }

    private void DetectGrounded(Vector3 startPos, float checkDist, LayerMask groundMask)
    {
        groundRay.origin = startPos;
        groundRay.direction = Vector3.down;

        int hits = Physics.RaycastNonAlloc(groundRay, groundHit, checkDist + 0.25f, groundMask);

        if (hits > 0)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }


    private void DetectTireGround(Vector3 startPos, float checkDist, LayerMask groundMask, GameObject tireObj)
    {
        //set the y position a little bit inside the car incase the car recives so much force that the base is on the ground
        Vector3 sPos = new Vector3(startPos.x, startPos.y + 0.1f, startPos.z);
        
        tireRay.origin = sPos;
        tireRay.direction = Vector3.down;
        int hits = Physics.RaycastNonAlloc(tireRay, raycastHits, checkDist + 0.1f, groundMask);

        if (hits > 0)
        {
            RaycastHit h = raycastHits[0];
            SnapGrounded(h, rb, startPos, targetHeight, frequency, damping);
            tireObj.transform.position = new Vector3(startPos.x, (h.point.y + tireRadius), startPos.z);
        }
        else
        {
            tireObj.transform.position = new Vector3(startPos.x, startPos.y - checkDist, startPos.z);

        }

        //raycast to check for ground
        //if (Physics.Raycast(sPos, Vector3.down, out RaycastHit hit, checkDist + 0.1f, groundMask))
        //{
        //    SnapGrounded(hit, rb, startPos, targetHeight, frequency, damping);
        //    tireObj.transform.position = new Vector3(startPos.x, (hit.point.y + tireRadius), startPos.z);
        //}
        
    }

    private void SnapGrounded(RaycastHit hit, Rigidbody rb, Vector3 forcePos, float floatHeight, float springStrength, float springDamping)
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 rayDir = transform.TransformDirection(-transform.up);

        float rayVel = Vector3.Dot(rayDir, vel);

        //the difference between where our raycast hit the ground and where we want to be floating
        //for example raycast is 2m long but we want to be floating 0.5m above the ground so our difference is 1.5
        float targetY = hit.distance - floatHeight;

        //apply our spring force, makes the player adjust their position by applying force in the direction that leads us towards the desired float height
        //the damping value slows down the bobbing until it stops
        float springForce = (targetY * springStrength) - (rayVel * springDamping);

        //add this force multiplied by our desired direction to our rigidbody
        rb.AddForceAtPosition((rayDir * springForce) * rb.mass, forcePos);
    }

    private void CalculateMovement(Rigidbody rb, Vector3 dir, float accelValue, float maxSpeed, float maxAccelStep)
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetDir = dir;

        float targetSpeed = maxSpeed;

        //this is the speed we are trying to reach / our maximum speed with a direction provided by a camera dependant input
        Vector3 targetVelocity = (targetDir * (targetSpeed));

        //our current desired velocity direction
        Vector3 unitVel = goalVelocityChange.normalized;

        //the difference between our new target direction and our current target direction
        float velDot = Vector3.Dot(targetDir, unitVel);

        float accel = accelValue;

     
        //how much we will change our velocity next step with smoothing by vector3.movetowards
        goalVelocityChange = Vector3.MoveTowards(goalVelocityChange, targetVelocity, accel * 0.02f);

        //the amount of velocity change needed to reach our maximum velocity
        Vector3 velocityChange = (goalVelocityChange - currentVel) / 0.02f;

        //maxAccelStep limits how much our velocity can change per step
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxAccelStep);

        //apply our force to our velocity
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        //apply our velocity to the rigidbody
        rb.AddForce(velocityChange * rb.mass);
    }
}
