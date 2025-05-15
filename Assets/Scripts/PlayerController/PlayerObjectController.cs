using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectController : MonoBehaviour
{
    [Header("Pickup Variables")]
    [SerializeField] private float pickupRadius; //how far away the player can pick up objects
    [SerializeField] private bool showRadius = false; //show the radius in inspector
    [SerializeField] private Transform holdPoint; //where the object is held in world 
    [SerializeField] private float pickupSpeed = 1; //how fast the object moves to the hold point when picking up an object
    [SerializeField] private LayerMask grabbableMask; //what layers can be grabbed
    [SerializeField] private PlayerInputDetection inputDetection;

    [Space, Header("Throwing Variables")]
    [SerializeField] private float forwardThrowForce = 8; //how much force forwards to add to an object when throwing
    [SerializeField] private float upwardsThrowForce = 4;//how much force upwards to add to an object when throwing

    [Space, Header("Debugging")]
    [SerializeField] public GameObject currentObject = null; //the object that the player is currently holding

    //Variables accessible by different states
    public float PickupRadius { get { return pickupRadius; } }
    public Transform HoldPoint { get { return holdPoint; } }
    public float PickupSpeed { get { return pickupSpeed; } }
    public LayerMask GrabbableMask {  get { return grabbableMask; } }
    public float ForwardThrowForce { get { return forwardThrowForce; } }
    public float UpwardsThrowForce { get {return upwardsThrowForce; } }

    private void Awake()
    {
        if (inputDetection == null)
        {
            inputDetection = GetComponent<PlayerInputDetection>();
            Debug.Log("Input detection set to get component by default");
        }
    }
    private void Start()
    {
      
    }
    private void OnDrawGizmos()
    {
        if (showRadius)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
        }
    }

    public bool GetPickupInput()
    {
        return inputDetection.grabPressed;
    }

    public bool GetThrowInput()
    {
        return inputDetection.attackPressed;
    }

}
