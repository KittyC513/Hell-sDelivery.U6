using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    private static ItemBase instance;
    public static ItemBase Instance => instance;

    private bool withPickupRange_p1 = false;
    private bool withPickupRange_p2 = false;

    [HideInInspector] public PlayerLockOn playerLockOn;
    [HideInInspector] public PlayerInputDetection inputDetection;

    [SerializeField]
    protected ItemHandler itemHandler;

    [SerializeField] public Vector3 holdRotation = Vector3.zero;
    [SerializeField] public Vector3 bagRotation = Vector3.zero;

    private bool isAvaliable = true;

    [HideInInspector] public bool isOnUse = false;

    private Rigidbody rb;
    public RigidbodyConstraints rbContraints;

    protected virtual void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rbContraints = rb.constraints;
        }
    }
    protected void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        // Initialization logic can be added here if needed
    }

    public float pickupRadium = 1f;
    Collider[] colliders;

    protected void Update()
    {
        //1. check if it's not picked up
        if (isAvaliable)
        {
            DetectInPickUpRange();
            PickUp();
        }

        if(isOnUse)
        {
            if(inputDetection.crouchPressed)
            {
                UseFunction();
            }
        }

    }
    private void FixedUpdate()
    {

    }

    public virtual void DetectInPickUpRange()
    {
        //2. check if in pickup range
        colliders = Physics.OverlapSphere(this.transform.position, pickupRadium, 1 << LayerMask.NameToLayer("HitCollider_p1") | 1 << LayerMask.NameToLayer("HitCollider_p2"));
        print(colliders.Length);

        switch (colliders.Length)
        {
            case 0:
                withPickupRange_p1 = false;
                withPickupRange_p2 = false;
                break;

            case 1:
                if (colliders[0].gameObject.layer == LayerMask.NameToLayer("HitCollider_p1"))
                {
                    withPickupRange_p1 = true;
                }
                else if (colliders[0].gameObject.layer == LayerMask.NameToLayer("HitCollider_p2"))
                {
                    withPickupRange_p2 = true;
                }
                break;

            case 2:
                withPickupRange_p1 = true;
                withPickupRange_p2 = true;
                break;
        }
    }
    public virtual void PickUp()
    {
        //3. press button to pick up
        if(withPickupRange_p1 && GameManager.instance.InputDetection_p1.grabPressed)
        {

            if (GameManager.instance.bag_p1.bag.Count < 2)
            {
                if (itemHandler != null) itemHandler.EquipItem(GameManager.instance.itemControl_p1);

                isAvaliable = false;
                GameManager.instance.bag_p1.AddItem(this);

                playerLockOn = GameManager.instance.player1.GetComponent<PlayerLockOn>();
                inputDetection = GameManager.instance.InputDetection_p1;
                print("Added to bag");
            }
            else
            {
                print("p1 bag is full");
            }

        }
        else if(withPickupRange_p2 && GameManager.instance.InputDetection_p2.grabPressed)
        {
            if(GameManager.instance.bag_p2.bag.Count < 2)
            {
                if (itemHandler != null) itemHandler.EquipItem(GameManager.instance.itemControl_p2);

                isAvaliable = false;
                GameManager.instance.bag_p2.AddItem(this);
                print("Added to bag");

                playerLockOn = GameManager.instance.player2.GetComponent<PlayerLockOn>();
                inputDetection = GameManager.instance.InputDetection_p2;
            }
            else
            {
                print("p2 bag is full");
            }
        }

    }

    public virtual void Throw()
    {
        if (isOnUse)
        {

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, pickupRadium);
    }

    public virtual void UseFunction()
    {

    }

}
