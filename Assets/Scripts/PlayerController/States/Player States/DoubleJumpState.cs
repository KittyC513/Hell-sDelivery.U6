using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Rigidbody rb;

    private float jumpTime = 0;
    private float maxJumpTime = 1.7f;

    private float jumpHeight;
    private float jumpDecay;

    private float gravityFactor = 1;


    public DoubleJumpState(PlayerStateMachine.PlayerStates key, PlayerController playerController) : base(key)
    {
        pControl = playerController;
    }


    public override void EnterState()
    {
        //setup our rigidbody
        rb = pControl.RB;
        gravityFactor = 1;

        //set our jump height and decay variables up by grabbing from our player controller
        jumpHeight = pControl.DoubleJumpHeight;
        jumpDecay = pControl.DoubleJumpDecayRate;

        //reset the jump time to 0
        jumpTime = 0;

        //make sure our y velocity does not affect our current jump by setting it to 0
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        //calculate how much force we need to reach our desired jump height in unity meters
        float jumpForce = Mathf.Sqrt(jumpHeight * (-jumpDecay) * -2) * rb.mass;


        //add our jump force as an impluse force as we activate this script
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        
        //if the player has left the ground without doing an initial jump make sure that jump is taken out of the count
        if (pControl.remainingJumps == pControl.MaxJumps)
        {
            pControl.remainingJumps = pControl.MaxJumps - 1;
        }

        pControl.remainingJumps -= 1;

    }

    public override void ExitState()
    {
        jumpTime = 0;
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        if (rb.linearVelocity.y <= 0 && jumpTime > 0.05f)
        {
            //if our jump is no longer providing upwards force transition to falling
            return PlayerStateMachine.PlayerStates.airborne;

        }
        if (pControl.Grounded && jumpTime > 0.05f)
        {
            //if we touch the ground make us grounded
            return PlayerStateMachine.PlayerStates.grounded;
        }

        if (pControl.remainingJumps < 0)
        {
            return PlayerStateMachine.PlayerStates.airborne;
        }

        //TEMPORARY
        if (pControl.DetectCrouchInput() && pControl.CheckCanAttack() && jumpTime > 0.1f)
        {
            return PlayerStateMachine.PlayerStates.attack;
        }

        return stateKey;
    }

    public override void UpdateState()
    {
        jumpTime += Time.deltaTime;
        ChangeGravity(pControl.JumpReleaseFactor, pControl.JumpPeakFactor);
    }

    public override void PhysicsUpdate()
    { 
        //add up the time our jump has been active
        JumpCalculations();
    }

    private void ChangeGravity(float releaseFactor, float peakFactor)
    {

        //if the player releases the jump button early increase our gravity to cut the jump
        if (!pControl.DetectJumpHold() && jumpTime > 0.1f && jumpTime <= maxJumpTime)
        {
            gravityFactor = releaseFactor;
        }
        else
        {
            gravityFactor = 1;
        }

        //if we are near the peak of our jump slow down our gravity to give us extra air time
        if (rb.linearVelocity.y <= pControl.JumpPeakRange)
        {
            gravityFactor = peakFactor;
        }
    }

    private void JumpCalculations()
    {
        //decay our upwards velocity by our decay rate, a faster decay rate means a faster jump upwards
        rb.AddForce((-jumpDecay * gravityFactor) * Vector3.up);
    }
}
