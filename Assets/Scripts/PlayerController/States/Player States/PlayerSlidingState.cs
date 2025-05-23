using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlidingState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Rigidbody rb;

    private Vector3 goalVelocityChange;

    public PlayerSlidingState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
    }

    public override void EnterState()
    {
        goalVelocityChange = Vector3.zero;
        rb = pControl.RB;

        //set animation 
        animName = "Player_Slide";

        //Debug.Log(pControl.GroundAngle);
        //Debug.Log("SLIDING");
    }

    public override void ExitState()
    {
        
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        //if we are no longer on a slope return to grounded
        if (pControl.GroundAngle <= 5)
        {
            return PlayerStateMachine.PlayerStates.grounded;
        }

        //if we are jumping, grounded and are not on a slope too steep we can jump
        if (pControl.DetectJumpInput() && pControl.Grounded && pControl.GroundAngle <= pControl.MaxSlopeAngle)
        {
            return PlayerStateMachine.PlayerStates.jump;
        }

        //if we are no longer grounded become airborne
        if (!pControl.Grounded)
        {
            return PlayerStateMachine.PlayerStates.airborne;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        
    }

    public override void PhysicsUpdate()
    {
        //using the same calculate movement function but passing in the slope direction to add force along the slope
        CalculateMovement(rb, pControl.SlopeSlideVelocity(pControl.DetectGround()));
    }

    private void CalculateMovement(Rigidbody rb, Vector3 direction)
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetDir = direction.normalized;

        //this is the speed we are trying to reach / our maximum speed with a direction provided by a camera dependant input
        Vector3 targetVelocity = targetDir * (pControl.MaxSlideSpeed);

        
        float accelFactor = pControl.GroundAngle / 45;
        
        //how much we will change our velocity next step with smoothing by vector3.movetowards
        goalVelocityChange = Vector3.MoveTowards(goalVelocityChange, targetVelocity, (pControl.SlideAccel * accelFactor) * 0.02f);

        //the amount of velocity change needed to reach our maximum velocity
        Vector3 velocityChange = (goalVelocityChange - currentVel) / 0.02f;

        //maxAccelStep limits how much our velocity can change per step
        velocityChange = Vector3.ClampMagnitude(velocityChange, pControl.MaxSlideAccelStep);

        //apply our force to our velocity
        rb.AddForce(velocityChange * rb.mass);
    }

}
