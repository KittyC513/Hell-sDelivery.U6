using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class PickupPickingState : BaseState<PickupStateMachine.PickupStates>
{

    private PlayerObjectController oControl;
    private Rigidbody rb;

    private float lerpTime = 0;
    private Vector3 objectStartPos;
    private float pickupSpeed;


    public PickupPickingState(PickupStateMachine.PickupStates key, PlayerObjectController controller) : base(key)
    {
        oControl = controller;
        pickupSpeed = oControl.PickupSpeed;
    }

    public override void EnterState()
    {
        //get the rigidbody of our object
        rb = oControl.currentObject.GetComponent<Rigidbody>();

        Debug.Log("ENTER PICKUP");

        //reset our lerp time to start the lerp
        lerpTime = 0;

        //get the position that our object started at for the lerp
        objectStartPos = oControl.currentObject.transform.position;
    }

    public override void ExitState()
    {

    }

    public override PickupStateMachine.PickupStates GetNextState()
    {
        //if we have reached the final lerp time or our object has reached destination switch to holding the object
        if (lerpTime >= 1)
        {
            return PickupStateMachine.PickupStates.holding;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        //update our lerp time
        lerpTime += pickupSpeed * Time.deltaTime;

        //lerp the object from the starting point to where the player holds it
        oControl.currentObject.transform.position = Vector3.Lerp(objectStartPos, oControl.HoldPoint.transform.position, lerpTime);

        //this doesnt work at the moment
        //pControl.LimitPlayerVelocity(0f, this.ToString());
     
    }

}
