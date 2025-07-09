using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase<T> : MonoBehaviour where T:class
{
    private static T instance;
    public static T Instance => instance;

    private bool withPickupRange_p1 = false;
    private bool withPickupRange_p2 = false;

    [SerializeField]
    protected ItemHandler itemHandler;

    private bool isAvaliable = true;

    protected virtual void Awake()
    {
        instance = this as T;
    }
    protected void Start()
    {
        
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
            itemHandler.EquipItem(GameManager.instance.itemControl_p1);
            isAvaliable = false;
        }
        else if(withPickupRange_p2 && GameManager.instance.InputDetection_p2.grabPressed)
        {
            itemHandler.EquipItem(GameManager.instance.itemControl_p2);
            isAvaliable = false;
        }

    }

    public virtual void Throw()
    {
        if (OnUse())
        {
            //throw function
        }
    }


    public virtual bool OnUse()
    {
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, pickupRadium);
    }

}
