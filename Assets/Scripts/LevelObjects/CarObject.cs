using System;
using UnityEditor.ShaderGraph.Internal;
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

    //how do i apply force in 4 spots for each tire???

   //imagine the object is standing on the 4 tires
   //each point is holding a portion of the car up

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //snap to the ground at each tire position
        DetectGround(frontLTire.position, targetHeight + 1f, groundMask, frontLTire.GetChild(0).gameObject);
        DetectGround(frontRTire.position, targetHeight + 1f, groundMask, frontRTire.GetChild(0).gameObject);
        DetectGround(backLTire.position, targetHeight + 1f, groundMask, backLTire.GetChild(0).gameObject);
        DetectGround(backRTire.position, targetHeight + 1f, groundMask, backRTire.GetChild(0).gameObject);

        //clamp the velocity to stop the car gaining any massive forces from weird physics interactions
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, frequency*4);

        //clamp the rotation so that the car won't flip over
        rb.rotation = Quaternion.Euler(Mathf.Clamp(rb.rotation.x, -maxRotationAngles.x, maxRotationAngles.x), Mathf.Clamp(rb.rotation.y, -maxRotationAngles.y, maxRotationAngles.y),
        Mathf.Clamp(rb.rotation.z, -maxRotationAngles.z, maxRotationAngles.z));
    }


    private void DetectGround(Vector3 startPos, float checkDist, LayerMask groundMask, GameObject tireObj)
    {
        //set the y position a little bit inside the car incase the car recives so much force that the base is on the ground
        Vector3 sPos = new Vector3(startPos.x, startPos.y + 0.25f, startPos.z);

        //raycast to check for ground
        if (Physics.Raycast(sPos, Vector3.down, out RaycastHit hit, checkDist + 0.25f, groundMask))
        {
            SnapGrounded(hit, rb, startPos, targetHeight, frequency, damping);
            tireObj.transform.position = new Vector3(startPos.x, (hit.point.y + tireRadius), startPos.z);
        }
        else
        {
            tireObj.transform.position = new Vector3(startPos.x, startPos.y - checkDist, startPos.z);
        }
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
        rb.AddForceAtPosition(rayDir * springForce, forcePos);
    }
}
