using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupThrowingState : BaseState<PickupStateMachine.PickupStates>
{
    private PlayerObjectController oControl;
    private Rigidbody rb;

    float throwForce;
    float upwardsThrowForce;

    private bool thrown = false;


    public PickupThrowingState(PickupStateMachine.PickupStates key, PlayerObjectController controller) : base(key)
    {
        oControl = controller;
    }

    public override void EnterState()
    {
        //get our throw variables from the object controller
        throwForce = oControl.ForwardThrowForce;
        upwardsThrowForce = oControl.UpwardsThrowForce;

        //reset our thrown variable
        thrown = false;

        //get the rigidbody attached to the current object
        rb = oControl.currentObject.GetComponent<Rigidbody>();

        //get the forwards direction of our player
        Vector3 throwDirection = oControl.HoldPoint.TransformDirection(Vector3.forward);

        //calculate our direction with our forces added
        Vector3 velDirection = new Vector3(throwDirection.x * throwForce, upwardsThrowForce, throwDirection.z * throwForce);

        //reset the objects velocity just in case
        rb.linearVelocity = Vector3.zero;

        //add force to the object
        rb.AddForce(velDirection, ForceMode.Impulse);

        //we have now thrown and can exit
        thrown = true;
    }

    public override void ExitState()
    {
        //we are no longer holding an object
        oControl.currentObject = null;

        //reset our thrown variable
        thrown = false;
    }

    public override PickupStateMachine.PickupStates GetNextState()
    {
        if (thrown)
        {
            return PickupStateMachine.PickupStates.empty;
        }
        return stateKey;
    }

    public override void UpdateState()
    {

    }
}
