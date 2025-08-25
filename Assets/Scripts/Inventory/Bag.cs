using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Bag : MonoBehaviour
{
    public List<ItemBase> bag;

    public PlayerInputDetection inputDetection;

    public Transform equipPoint;
    public Transform bagPoint;

    public float swapCooldown = 0.3f;
    public float swapTimer = 0f;

    [SerializeField] private GameObject playerModel;

    [HideInInspector] public ItemBase activeItemBase;
    private GameObject activeItem;
    private Rigidbody activeRB;
    private RigidbodyConstraints oldContraints;


    private Vector3 velocity;
    private Vector3 position;

    private Vector3 rotVelocity;

    private float damping = 0.65f;
    private float frequency = 45;

    private Vector3 targetRotation;
    private Vector3 springRotation;

    SpringUtils.tDampedSpringMotionParams tempX;
    SpringUtils.tDampedSpringMotionParams tempY;
    SpringUtils.tDampedSpringMotionParams tempZ;

    SpringUtils.tDampedSpringMotionParams rotX;
    SpringUtils.tDampedSpringMotionParams rotY;
    SpringUtils.tDampedSpringMotionParams rotZ;

    private Vector3 targetPos;

    [Header("Invisible State")]
    public bool isInvisible = false;
    public GameObject playerMod;

    private void Awake()
    {
        bag = new List<ItemBase>();
        ResetSprings();
    }

    public void AddItem(ItemBase item)
    {
        bag.Add(item);
        OnEquipItem(item.gameObject);

        if (activeItem == null)
        {
            //if there is only one item in the bag, set it as the active item
            activeItemBase = item;
            activeItemBase.isOnUse = true;
            activeItem = item.gameObject;
            activeRB = activeItem.GetComponent<Rigidbody>();
        }

    }

    public void RemoveItem(ItemBase item)
    {
        item.transform.parent = null;
        item.isAvaliable = true;
        OnRemoveItem(item);
        bag.Remove(item);
    }

    private void Update()
    {
        EquipItem();

        //calculate all the spring motion values for xyz values of position
        SpringUtils.CalcDampedSpringMotionParams(ref tempX, Time.deltaTime, frequency, damping);
        SpringUtils.CalcDampedSpringMotionParams(ref tempY, Time.deltaTime, frequency, damping);
        SpringUtils.CalcDampedSpringMotionParams(ref tempZ, Time.deltaTime, frequency, damping);
        SpringUtils.UpdateDampedSpringMotion(ref position.x, ref velocity.x, targetPos.x, tempX);
        SpringUtils.UpdateDampedSpringMotion(ref position.y, ref velocity.y, targetPos.y, tempY);
        SpringUtils.UpdateDampedSpringMotion(ref position.z, ref velocity.z, targetPos.z, tempZ);
        //calculate all the spring motion values for xyz values of rotation
        SpringUtils.CalcDampedSpringMotionParams(ref rotX, Time.deltaTime, frequency, damping);
        SpringUtils.CalcDampedSpringMotionParams(ref rotY, Time.deltaTime, frequency, damping);
        SpringUtils.CalcDampedSpringMotionParams(ref rotZ, Time.deltaTime, frequency, damping);
        SpringUtils.UpdateDampedSpringMotion(ref springRotation.x, ref rotVelocity.x, targetRotation.x, rotX);
        SpringUtils.UpdateDampedSpringMotion(ref springRotation.y, ref rotVelocity.y, targetRotation.y, rotY);
        SpringUtils.UpdateDampedSpringMotion(ref springRotation.z, ref rotVelocity.z, targetRotation.z, rotZ);
    }

    public void FixedUpdate()
    {
        HoldItems();
    }

    public void EquipItem()
    {
        switch(bag.Count)
        {
            case 0:
                //if there are no items in the bag, set the active item to null
                activeItemBase = null;
                activeItem = null;
                activeRB = null;
                break;
            case 1:

                if (!bag[0].isOnUse)
                {
                    bag[0].transform.position = bagPoint.position;
                }

                if ((inputDetection.swapItemPressed_left || inputDetection.swapItemPressed_right) && swapTimer >= swapCooldown)
                {
                    //if the only item we have is in use then swap it to the back and leave the player empty handed
                    if (bag[0].isOnUse)
                    {
                        activeItemBase.OnItemSwap();

                        swapTimer = 0;
                        bag[0].isOnUse = false;
                        bag[0].GetComponent<Collider>().isTrigger = true;
                      
                        activeItemBase = null;
                        activeItem = null;
                        activeRB = null;

                        //reset the spring params
                        ResetSprings();
                    }
                    else //otherwise bring the only item from the bag to the hand
                    {
                        
                        swapTimer = 0;
                        bag[0].isOnUse = true;
                        bag[0].GetComponent<Collider>().isTrigger = true;

                        activeItemBase = bag[0];
                        activeItem = bag[0].gameObject;
                        activeRB = activeItem.GetComponent<Rigidbody>();
                       
                        //reset the spring params
                        ResetSprings();
                    }
                    
                }
                break;
            case 2:
                if(activeItemBase == bag[0])
                {
                    if ((inputDetection.swapItemPressed_left || inputDetection.swapItemPressed_right) && swapTimer >= swapCooldown)
                    {
                        activeItemBase.OnItemSwap();

                        swapTimer = 0;
                        bag[0].isOnUse = false;
                        bag[0].GetComponent<Collider>().isTrigger = true;

                        //switch to the second item in the bag
                        bag[1].isOnUse = true;
                        bag[1].GetComponent<Collider>().isTrigger = false;
                        activeItemBase = bag[1];
                        activeItem = bag[1].gameObject;
                        activeRB = activeItem.GetComponent<Rigidbody>();
                        //reset the spring params
                        ResetSprings();
                    }

                    #region Auto-switch
                    if (bag[1].autoSwitch)
                    {
                        activeItemBase.OnItemSwap();

                        bag[0].isOnUse = false;
                        bag[0].GetComponent<Collider>().isTrigger = true;

                        //switch to the second item in the bag
                        bag[1].isOnUse = true;
                        bag[1].GetComponent<Collider>().isTrigger = false;
                        activeItemBase = bag[1];
                        activeItem = bag[1].gameObject;
                        activeRB = activeItem.GetComponent<Rigidbody>();
                        //reset the spring params
                        ResetSprings();
                        bag[1].autoSwitch = false;
                        Debug.Log(bag[1].autoSwitch);
                    }
                    #endregion


                    bag[1].transform.position = bagPoint.position;
                }

                if (activeItemBase == bag[1])
                {
                    if ((inputDetection.swapItemPressed_left || inputDetection.swapItemPressed_right) && swapTimer >= swapCooldown)
                    {
                        activeItemBase.OnItemSwap();

                        swapTimer = 0;
                        bag[1].isOnUse = false;
                        bag[1].GetComponent<Collider>().isTrigger = true;
                        //bag[1].transform.position = bagPoint.position;

                        //switch to the second item in the bag
                        bag[0].isOnUse = true;
                        bag[0].GetComponent<Collider>().isTrigger = false;
                        activeItemBase = bag[0];
                        activeItem = bag[0].gameObject;
                        activeRB = activeItem.GetComponent<Rigidbody>();

                        //reset the spring params
                        ResetSprings();
                    }

                    #region Auto-switch
                    if (bag[0].autoSwitch)
                    {
                        activeItemBase.OnItemSwap();

                        bag[0].isOnUse = false;
                        bag[0].GetComponent<Collider>().isTrigger = true;

                        //switch to the second item in the bag
                        bag[1].isOnUse = true;
                        bag[1].GetComponent<Collider>().isTrigger = false;
                        activeItemBase = bag[1];
                        activeItem = bag[1].gameObject;
                        activeRB = activeItem.GetComponent<Rigidbody>();
                        //reset the spring params
                        ResetSprings();
                        bag[0].autoSwitch = false;
                        Debug.Log(bag[0].autoSwitch);
                    }
                    #endregion
                    bag[0].transform.position = bagPoint.position;
                }

                break;
        }
        // If the swap button is not pressed, start the timer
        if (!inputDetection.swapItemPressed_left && !inputDetection.swapItemPressed_right)
        {
            if (swapTimer < swapCooldown)
            {
                swapTimer += Time.deltaTime;
            }
            else
            {
                swapTimer = swapCooldown;
            }

        }                              
    }

    private void OnRemoveItem(ItemBase item)
    {
        //reset the rigidbody contraints on dropping the item
        item.GetComponent<Rigidbody>().constraints = item.rbContraints;
        Physics.IgnoreCollision(this.GetComponent<Collider>(), item.GetComponent<Collider>(), false);
        item.GetComponent<Collider>().isTrigger = false;
        activeItem = null;
        item.isOnUse = false;
    }

    private void OnEquipItem(GameObject obj)
    {
        //ignore collision between the player holding the object and the object they hold
        Physics.IgnoreCollision(this.GetComponent<Collider>(), obj.GetComponent<Collider>(), true);
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void ResetSprings()
    {
        tempX = new SpringUtils.tDampedSpringMotionParams();
        tempY = new SpringUtils.tDampedSpringMotionParams();
        tempZ = new SpringUtils.tDampedSpringMotionParams();

        rotX = new SpringUtils.tDampedSpringMotionParams();
        rotY = new SpringUtils.tDampedSpringMotionParams();
        rotZ = new SpringUtils.tDampedSpringMotionParams();
        velocity = Vector3.zero;
        rotVelocity = Vector3.zero;
    }

    public void HoldItems()
    {
        if (activeItem != null && activeRB != null)
        {
            //set the active item to the hold point
            //Vector3 position = equipPoint.position;

            Vector3 endPos = activeItem.GetComponent<Collider>().ClosestPoint(this.transform.position);
            Vector3 center = activeItem.transform.position;

            float dist = Vector3.Distance(center, endPos);
            targetPos = equipPoint.position + (dist * this.transform.forward);

            // Get the delta position  
            Vector3 dirDelta = position - activeRB.position;
            // Get the velocity required to reach the target in the next frame
            dirDelta /= Time.fixedDeltaTime;
            // Clamp that to the max speed
            dirDelta = Vector3.ClampMagnitude(dirDelta, 75);

            activeRB.linearVelocity = dirDelta;
        }

        if (activeItemBase != null)
        {
            Vector3 itemRot = activeItemBase.holdRotation;
            //targetRotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(new Vector3(itemRot.x, itemRot.y, itemRot.z));
            targetRotation = playerModel.transform.forward;
            activeItem.transform.rotation = Quaternion.LookRotation(springRotation) * Quaternion.Euler(new Vector3(itemRot.x, itemRot.y, itemRot.z));
        }

        if (bag.Count > 0)
        {
            //if the first item is in the bag
            if (!bag[0].isOnUse)
            {
                Vector3 itemRot = bag[0].bagRotation;
                bag[0].transform.rotation = Quaternion.LookRotation(playerModel.transform.forward) * Quaternion.Euler(new Vector3(itemRot.x, itemRot.y, itemRot.z));
            }
            else if (bag.Count > 1) //otherwise if the bag count is greater than 1 that means the other item is in the bag
            {
                Vector3 itemRot = bag[1].bagRotation;
                bag[1].transform.rotation = Quaternion.LookRotation(playerModel.transform.forward) * Quaternion.Euler(new Vector3(itemRot.x, itemRot.y, itemRot.z));
            }
        }
    }

}

