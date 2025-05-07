using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupEmptyState : BaseState<PickupStateMachine.PickupStates>
{
    private PlayerObjectController oControl;
    private Collider[] inRangeObjects;
    public PickupEmptyState(PickupStateMachine.PickupStates key, PlayerObjectController controller) : base(key)
    {
        oControl = controller;
        
    }

    public override void EnterState()
    {
        //Debug.Log("ENTER EMPTY");
    }

    public override void ExitState()
    {
        
    }

    public override PickupStateMachine.PickupStates GetNextState()
    {
        //this will be changed with the new input system at a later date
        if (Input.GetKeyDown(KeyCode.F))
        {
            //get our detected gameobject
            GameObject temp = DetectObject();

            //if there is no object do nothing
            if (temp != null)
            {
                //otherwise set our current object and start to pick it up
                oControl.currentObject = temp;
                return PickupStateMachine.PickupStates.pickup;
            }
        }
        return stateKey;
    }

    public override void UpdateState()
    {
       
    }

    private GameObject DetectObject()
    {
        //grab an array of all in range objects that can be picked up
        inRangeObjects = Physics.OverlapSphere(oControl.transform.position, oControl.PickupRadius, oControl.GrabbableMask);

        if (inRangeObjects.Length > 0 )
        {
            //set our current shortest distance and target collider
            float shortestDist = 100;
            Collider target = null;

            foreach (Collider collider in inRangeObjects)
            {
                //detect the current distance to the next object in the array
                float currentDist = Vector3.Distance(oControl.transform.position, collider.transform.position);

                //this makes sure the player picks up the closest object to them
                if (currentDist < shortestDist)
                {
                    shortestDist = currentDist;
                    target = collider;
                }
            }
            //return the closest object
            return target.gameObject;
        }
        else
        {
            return null;
        }
    }
}

