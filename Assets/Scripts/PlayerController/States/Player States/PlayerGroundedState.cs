using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Rigidbody rb;
    private Vector3 lastFramePos;

    private bool movingPlat = false;
    private Transform startTransform;
    private Vector3 vel;

    public PlayerGroundedState(PlayerStateMachine.PlayerStates key, PlayerController playerController) : base(key)
    {
        pControl = playerController;
        startTransform = pControl.transform.parent;
    }

    public override void EnterState()
    {
        pControl.gravityScale = 1f;

        if(pControl.RB != null)
            rb = pControl.RB;

        //reset our maximum jumps
        pControl.remainingJumps = pControl.MaxJumps;

        //set placeholder animation
        animName = "Idle";
    }

    public override void ExitState()
    {
        //Debug.Log("EXIT GROUNDED");
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
            //if the player jumps off a moving platform apply their movement + the platforms movement to the player to add inertia 
            if (pControl.GroundObject.CompareTag("MovingPlat"))
            {
                rb.AddForce(vel, ForceMode.Impulse);
            }
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
       if (pControl.DetectAttackInput() && pControl.CheckCanAttack())
       {
             return PlayerStateMachine.PlayerStates.attack;
       }

        if (pControl.GroundObject.CompareTag("BouncePad"))
        {
            pControl.GroundObject.GetComponent<BouncePad>().BounceObject(rb, rb.linearVelocity.y);
            return PlayerStateMachine.PlayerStates.jump;
        }



        return stateKey;
    }

    //could have a way to deal with slope sliding in grounded
    //sliding as in the slope is too steep
    //or i could have a state where the player slides down slopes and just trigger that if the slope is too steep

    //i want the player to be able to walk up the slope but slowly lose velocity until they get low enough that they start sliding down

    //lets make a sliding function

    public override void UpdateState()
    {
        SnapGrounded(pControl.DetectGround());
    }

    public override void PhysicsUpdate()
    {
        CalculateObjectVelocity();
    }

    private void CalculateObjectVelocity()
    {
        //get the position on the current frame
        Vector3 thisFramePos = pControl.transform.position;

        //calc in here//

        //get the distance between the position last frame and this frame
        float dist = Vector3.Distance(thisFramePos, lastFramePos);

        //get the direction the player is moving in
        Vector3 dir = (thisFramePos - lastFramePos).normalized;

        //calcs end//

        //after calculating set the last frame to the current frame so that next calc will use it 
        lastFramePos = thisFramePos;

        //the players current movement speed, how far they move over a fixed update interval (0.02f)
        float spd = dist / 0.02f;

        //velocity (a direction with speed value)
        vel = (dir * spd);
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
