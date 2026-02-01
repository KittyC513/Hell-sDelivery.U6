using UnityEngine;

public enum E_PickupType
{
    one,
    two,
    three,
}
public class ItemBase : MonoBehaviour
{
    private static ItemBase instance;
    public static ItemBase Instance => instance;

    [SerializeField] private bool withPickupRange_p1 = false;
    [SerializeField] private bool withPickupRange_p2 = false;

    [HideInInspector] public PlayerLockOn playerLockOn;
    [HideInInspector] public PlayerInputDetection inputDetection;
    private PlayerSFX playerSFX;
    protected Bag currentBag;
    [SerializeField] private bool usePhysics = false;
    //[SerializeField]
    //protected ItemHandler itemHandler;

    public Vector3 holdRotation = Vector3.zero;
    public Vector3 bagRotation = Vector3.zero;

    [SerializeField]
    public bool isAvaliable = true;

    [SerializeField] private BillboardUI pickupIcon;

    protected ThrowArc throwArc;

    [HideInInspector] public bool isOnUse = false;

    [SerializeField]
    private Rigidbody rb;
    public RigidbodyConstraints rbContraints;

    //throwing variables
    [Space, Header("Throwing Variables")]
    [SerializeField] private float maxButtonHoldTime = 2f;
    [SerializeField] private float buttonHoldForce = 18;
    [SerializeField] private float throwHoldTimer = 0.2f;
    private float baseThrowForce = 2;
    private float yStartForce = 0.8f;
    private float yEndForce = 0.15f;
    protected float gravity;

    private float buttonHoldTime = 0;
    private Vector3 throwDirection = Vector3.zero;
    private float pickupCooldown = 0.45f;
    private float pickupTime;

    private bool usePressed = false;

    public E_PickupType pickupType = E_PickupType.two;
    public bool autoSwitch = false;
    

    protected virtual void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rbContraints = rb.constraints;
        }

        CustomGravity temp = GetComponent<CustomGravity>();

        if (temp != null)
        {
            gravity = temp.gravity;
        }
        else
        {
            gravity = Physics.gravity.y;
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

    protected virtual void Update()
    {
        //1. check if it's not picked up
        if (isAvaliable)
        {
            DetectInPickUpRange();
            PickUp();

            if (!usePhysics)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            rb.isKinematic = false;

            if (pickupIcon != null)
            { 
                pickupIcon.ShowIconToPlayer(false, 2);
                pickupIcon.ShowIconToPlayer(false, 1);
            }

            Throw();
        }

        if (isOnUse)
        {
            if (inputDetection.crouchPressed)
            {
                if (!usePressed)
                {
                    UseFunction();
                }
                usePressed = true;
                
            }
            else
            {
                usePressed = false;
            }
        }

    }

    public virtual void DetectInPickUpRange()
    {
        //2. check if in pickup range
        colliders = Physics.OverlapSphere(this.transform.position, pickupRadium, 1 << LayerMask.NameToLayer("HitCollider_p1") 
                                                                               | 1 << LayerMask.NameToLayer("HitCollider_p2"));

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

        //update the pickup icon
        if (pickupIcon != null && isAvaliable)
        {
            if (GameManager.instance.player1 != null)
            {
                if (withPickupRange_p1 && GameManager.instance.bag_p1.bag.Count < 2)
                {
                    pickupIcon.ShowIconToPlayer(true, 1);
                }
                else
                {
                    pickupIcon.ShowIconToPlayer(false, 1);
                }
            }

            if (GameManager.instance.player2 != null)
            {
                if (withPickupRange_p2 && GameManager.instance.bag_p2.bag.Count < 2)
                {
                    pickupIcon.ShowIconToPlayer(true, 2);
                }
                else
                {
                    pickupIcon.ShowIconToPlayer(false, 2);
                }
            }

        }
      
    }
    public virtual void PickUp()
    {
        //3. press button to pick up
        if(withPickupRange_p1 && GameManager.instance.InputDetection_p1.grabPressed)
        {
            switch (pickupType)
            {
                case E_PickupType.one:

                    if (GameManager.instance.bag_p1.bag.Count < 2 && GameManager.instance.bag_p1.activeItemBase == null)
                    {
                        //if (itemHandler != null) itemHandler.EquipItem(GameManager.instance.itemControl_p1);
                        isAvaliable = false;
                        GameManager.instance.bag_p1.AddItem(this);
                        
                        playerLockOn = GameManager.instance.player1.GetComponent<PlayerLockOn>();
                        
                        inputDetection = GameManager.instance.InputDetection_p1;
                        currentBag = GameManager.instance.bag_p1;
                        this.transform.SetParent(GameManager.instance.bag_p1.transform);

                        pickupTime = Time.time;
                        print("Added to p1's bag");
                    }
                    else
                    {
                        print("p1 bag is full");
                    }
                    break;

                case E_PickupType.two:
                    
                    if (GameManager.instance.bag_p1.bag.Count < 2)
                    {
                        isAvaliable = false;
                        GameManager.instance.bag_p1.AddItem(this);

                        playerLockOn = GameManager.instance.player1.GetComponent<PlayerLockOn>();
                        inputDetection = GameManager.instance.InputDetection_p1;
                        currentBag = GameManager.instance.bag_p1;
                        this.transform.SetParent(GameManager.instance.bag_p1.transform);

                        pickupTime = Time.time;
                        print("Added to p1's bag");

                        if(GameManager.instance.bag_p1.activeItemBase != null)
                        {
                            autoSwitch = true;
                        }
                    }
                    else
                    {
                        print("p1 bag is full");
                    }
                    break;
                case E_PickupType.three:
                    break;
                default:
                    break;
            }

            

        }
        else if(withPickupRange_p2 && GameManager.instance.InputDetection_p2.grabPressed)
        {
            if(GameManager.instance.bag_p2.bag.Count < 2 && GameManager.instance.bag_p2.activeItemBase == null)
            {
                //if (itemHandler != null) itemHandler.EquipItem(GameManager.instance.itemControl_p2);
                isAvaliable = false;
                GameManager.instance.bag_p2.AddItem(this);
                currentBag = GameManager.instance.bag_p2;
                playerLockOn = GameManager.instance.player2.GetComponent<PlayerLockOn>();
                inputDetection = GameManager.instance.InputDetection_p2;

                this.transform.SetParent(GameManager.instance.bag_p2.transform);

                pickupTime = Time.time;
                print("Added to p2's bag");
            }
            else
            {
                print("p2 bag is full");
            }
        }

    }

    //button hold time
    //force that correlates with the hold time
    //a default direction for now
    //possibly slow down the player
    //a base force
    //a multiplier based on the object
    //a line renderer that uses the force calculation to show a line + a maximum distance value
    public virtual void Throw()
    {
        if (isOnUse && Time.time - pickupTime >= pickupCooldown)
        {
            if (inputDetection.grabPressed)
            {
                if(inputDetection.grabCurrentTime >= throwHoldTimer)
                {
                    inputDetection.grabCurrentTime = throwHoldTimer;
                    throwArc = currentBag.gameObject.GetComponent<ThrowArc>();

                    //read how long the grab button is pressed for
                    if (buttonHoldTime < maxButtonHoldTime)
                    {
                        buttonHoldTime += Time.deltaTime;
                    }

                    buttonHoldTime = Mathf.Clamp(buttonHoldTime, 0, maxButtonHoldTime);

                    float throwForce = baseThrowForce + ((buttonHoldTime / maxButtonHoldTime) * buttonHoldForce);
                    Vector3 startArc = new Vector3(currentBag.transform.forward.x, yStartForce, currentBag.transform.forward.z).normalized;
                    Vector3 endArc = new Vector3(currentBag.transform.forward.x, yEndForce, currentBag.transform.forward.z).normalized;
                    Vector3 direction = Vector3.Lerp(startArc, endArc, (buttonHoldTime / maxButtonHoldTime));
                    Vector3 velocity = throwForce * direction;
                    Debug.Log(velocity);
                    throwArc.ShowThrowArc(velocity, transform.position, (buttonHoldTime / maxButtonHoldTime), gravity);
                }
                else
                {
                    inputDetection.grabCurrentTime += Time.deltaTime;
                }

            }
            else
            {
                //throw was held or pressed for some amount of time
                //throw the object
                if (buttonHoldTime > 0)
                {
                    throwArc.StopThrowArc();
                    float throwForce = baseThrowForce + ((buttonHoldTime / maxButtonHoldTime) * buttonHoldForce);
                    Vector3 startArc = new Vector3(currentBag.transform.forward.x, yStartForce, currentBag.transform.forward.z).normalized;
                    Vector3 endArc = new Vector3(currentBag.transform.forward.x, yEndForce, currentBag.transform.forward.z).normalized;
                    Vector3 direction = Vector3.Lerp(startArc, endArc, (buttonHoldTime / maxButtonHoldTime));
                    Vector3 velocity = throwForce * direction;

                    //remove from the player's bag
                    currentBag.RemoveItem(this);

                    //reset the velocity
                    rb.linearVelocity = Vector3.zero;

                    //add force to the object
                    rb.AddForce(velocity, ForceMode.Impulse);
                }

                buttonHoldTime = 0;
            }
        }
    }

    public void OnItemSwap()
    {
        buttonHoldTime = 0;
        if (throwArc != null) throwArc.StopThrowArc();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, pickupRadium);
    }

    public virtual void UseFunction()
    {

    }

}
