using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHoldingState : BaseState<PickupStateMachine.PickupStates>
{
    private Vector3 velocity;
    private Vector3 position;
    private float damping = 0.65f;
    private float frequency = 25;

    SpringUtils.tDampedSpringMotionParams tempX;
    SpringUtils.tDampedSpringMotionParams tempY;
    SpringUtils.tDampedSpringMotionParams tempZ;
    private Vector3 targetPos;

    private PlayerObjectController oControl;
    private Rigidbody objRb;

    private Transform oldParent;

    private RigidbodyConstraints constraints;
    public PickupHoldingState(PickupStateMachine.PickupStates key, PlayerObjectController controller) : base(key)
    {
        oControl = controller;
    }

    public override void EnterState()
    {
        //reset values used in the spring movement of the object while held
        velocity = Vector3.zero;
        position = oControl.currentObject.transform.position;
        tempX = new SpringUtils.tDampedSpringMotionParams();
        tempY = new SpringUtils.tDampedSpringMotionParams();
        tempZ = new SpringUtils.tDampedSpringMotionParams();

        objRb = oControl.currentObject.GetComponent<Rigidbody>();
        oldParent = oControl.currentObject.transform.parent;
        //set the objects parent so that it follows rotations
        oControl.currentObject.transform.SetParent(oControl.HoldPoint.transform);

        //ignore collision between the player holding the object and the object they hold
        Physics.IgnoreCollision(oControl.GetComponent<Collider>(), oControl.currentObject.GetComponent<Collider>(), true);

        //save the objects contraints to reset on exit
        constraints = objRb.constraints;

        //freeze rotation of the object
        objRb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
    }

    public override void ExitState()
    {
        if (oControl.currentObject != null)
        {
            if (oldParent != null)
            {
                oControl.currentObject.transform.SetParent(oldParent);
            }
            else
            {
                oControl.currentObject.transform.SetParent(null);
            }
          
            Physics.IgnoreCollision(oControl.GetComponent<Collider>(), oControl.currentObject.GetComponent<Collider>(), false);
            objRb.constraints = constraints;
        }
    }

    public override PickupStateMachine.PickupStates GetNextState()
    {
        if (oControl.GetThrowInput())
        {
            return PickupStateMachine.PickupStates.throwing;
        }


        if (Vector3.Distance(oControl.currentObject.transform.position, oControl.HoldPoint.position) > 5)
        {
            Physics.IgnoreCollision(oControl.GetComponent<Collider>(), oControl.currentObject.GetComponent<Collider>(), false);
            objRb.constraints = constraints;
            if (oldParent != null)
            {
                oControl.currentObject.transform.SetParent(oldParent);
            }
            else
            {
                oControl.currentObject.transform.SetParent(null);
            }
            oControl.currentObject = null;
            return PickupStateMachine.PickupStates.empty;
        }
        return stateKey;
    }

    public override void PhysicsUpdate()
    {
        // Get the delta position  
        Vector3 dir = position - oControl.currentObject.GetComponent<Rigidbody>().position;
        // Get the velocity required to reach the target in the next frame
        dir /= Time.fixedDeltaTime;
        // Clamp that to the max speed
        dir = Vector3.ClampMagnitude(dir, 65);

        //move the current object towards the target position (this does not ignore collision which can help with objects clipping through walls)
        if (oControl.currentObject != null)
        {
            objRb.linearVelocity = dir;
        }

    }

    public override void UpdateState()
    {
        if (oControl.currentObject != null)
        {
            
            Vector3 dir  = (position - objRb.position).normalized;
            Vector3 endPos = oControl.currentObject.GetComponent<Collider>().ClosestPoint(oControl.transform.position);
            Vector3 center = oControl.currentObject.transform.position;

            float dist = Vector3.Distance(center, endPos);
            targetPos = oControl.HoldPoint.position + (dist*oControl.transform.forward);
        }

        //calculate all the spring motion values for xyz values of position
        SpringUtils.CalcDampedSpringMotionParams(ref tempX, Time.deltaTime, frequency, damping);
        SpringUtils.CalcDampedSpringMotionParams(ref tempY, Time.deltaTime, frequency, damping);
        SpringUtils.CalcDampedSpringMotionParams(ref tempZ, Time.deltaTime, frequency, damping);
        SpringUtils.UpdateDampedSpringMotion(ref position.x, ref velocity.x, targetPos.x, tempX);
        SpringUtils.UpdateDampedSpringMotion(ref position.y, ref velocity.y, targetPos.y, tempY);
        SpringUtils.UpdateDampedSpringMotion(ref position.z, ref velocity.z, targetPos.z, tempZ);
    }



}
