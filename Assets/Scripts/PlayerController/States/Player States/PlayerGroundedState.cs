using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Rigidbody rb;
    public PlayerGroundedState(PlayerStateMachine.PlayerStates key, PlayerController playerController) : base(key)
    {
        pControl = playerController;
    }

    public override void EnterState()
    {
        //Debug.Log("ENTER GROUNDED");
        rb = pControl.RB;
        //rb.AddForce(rb.velocity * -1);

        //reset our maximum jumps
        pControl.remainingJumps = pControl.MaxJumps;
    }

    public override void ExitState()
    {
        Debug.Log("EXIT GROUNDED");
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        //if we don't detect ground set us to airborne
       if (!pControl.Grounded)
       {
            //if we transition from grounded to airborne (we walked off a ledge) start coyote time
            pControl.ResetCoyoteTime();
            return PlayerStateMachine.PlayerStates.airborne;
       }

       if (pControl.DetectJumpInput())
       {
            return PlayerStateMachine.PlayerStates.jump;
       }

       if (pControl.GroundAngle > pControl.MaxSlopeAngle)
       {
            //this prevents any weird angle detections making the character slide
            if (pControl.GroundAngle < 89.9f)
            return PlayerStateMachine.PlayerStates.sliding;
       }
       
       //if we are on a slope and input crouch start sliding
       if (pControl.GroundAngle > 15 && pControl.DetectCrouchInput())
       {
            if (pControl.GroundAngle < 89.9f)
                return PlayerStateMachine.PlayerStates.sliding;
       }

       //TEMPORARY
       //if (pControl.DetectCrouchInput() && pControl.canAttack)
       //{
       //     return PlayerStateMachine.PlayerStates.attack;
       //}
       return stateKey;
    }

    //could have a way to deal with slope sliding in grounded
    //sliding as in the slope is too steep
    //or i could have a state where the player slides down slopes and just trigger that if the slope is too steep

    //i want the player to be able to walk up the slope but slowly lose velocity until they get low enough that they start sliding down

    //lets make a sliding function

    public override void UpdateState()
    {
        //Debug.Log("ground update");
        //Debug.Log(pControl.RB.velocity);
        SnapGrounded(pControl.DetectGround());
    }

    public override void PhysicsUpdate()
    {
        
    }

    private void SnapGrounded(RaycastHit hit)
    {
        //this function makes our hitbox float above the ground
        //the reason to do this is to ensure our character does not get caught on any small geometry changes in the ground
        //it also adds a little spring animation that helps with game feel
        //Debug.Log(rb.velocity);
        Vector3 vel = rb.linearVelocity;
        Vector3 rayDir = pControl.transform.TransformDirection(-pControl.transform.up);

        float rayVel = Vector3.Dot(rayDir, vel);

        //the difference between where our raycast hit the ground and where we want to be floating
        //for example raycast is 2m long but we want to be floating 0.5m above the ground so our difference is 1.5
        float targetY = hit.distance - pControl.FloatHeight;

        //apply our spring force, makes the player adjust their position by applying force in the direction that leads us towards the desired float height
        //the damping value slows down the bobbing until it stops
        float springForce = (targetY * pControl.FloatHeightStrength) - (rayVel * pControl.FloatHeightDamping);

        //add this force multiplied by our desired direction to our rigidbody
        rb.AddForce(rayDir * springForce);
    }


}
