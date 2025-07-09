using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public float pickupRadium = 1f;
    Collider[] colliders;


    private void Update()
    {
        //1. check if it's not picked up
        if (!OnUse())
        {
            DetectInPickUpRange();
        }

    }

    public virtual void DetectInPickUpRange()
    {

        //2. check if in pickup range
        colliders = Physics.OverlapSphere(this.transform.position, pickupRadium, 1 << LayerMask.NameToLayer("HitCollider_p1") | 1 << LayerMask.NameToLayer("HitCollider_p2"));
        print(colliders.Length);
    }
    public virtual void PickUp()
    {
        //3. check if pickup button is pressed
        //if (colliders.Length > 0)
        //{
        //    for (int i = 0; i < colliders.Length; i++) 
        //    {
        //        inputDetections[i] = colliders[i].GetComponent<PlayerInputDetection>();
        //    }
        //}

    }

    public virtual void Throw()
    {

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
