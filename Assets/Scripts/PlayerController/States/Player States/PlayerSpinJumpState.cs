using UnityEngine;

public class PlayerSpinJumpState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Rigidbody rb;

    private float jumpTime = 0;
    private float maxJumpTime = 1.7f;

    private float jumpHeight;
    private float jumpDecay;

    private float gravityFactor = 1;
    private bool falling = false;

    private Quaternion startRotation;
    private Vector3 goalVelocityChange;
    public PlayerSpinJumpState(PlayerStateMachine.PlayerStates key, PlayerController playerController) : base(key)
    {
        pControl = playerController;
    }


    public override void EnterState(PlayerStateMachine.PlayerStates lastState)
    {
        //are we still jumping
        falling = false;

        if (pControl.playerNum == 1)
        {
            pControl.playerSfx.PlayShmonkJump();
        }
        else
        {
            pControl.playerSfx.PlayShminkJump();
        }

        //set our jump height and decay variables up by grabbing from our player controller
        jumpHeight = pControl.SpinJumpHeight;
        jumpDecay = pControl.SpinJumpDecay;

        if (pControl.PlayerModel != null)
        {
            //set the starting rotation to the rotation of the player right as this script starts
            startRotation = pControl.PlayerModel.transform.localRotation;
        }

        gravityFactor = 1;

        //reset the jump time to 0
        jumpTime = 0;

        //setup our rigidbody
        if (rb == null) rb = pControl.RB;

        //make sure our y velocity does not affect our current jump by setting it to 0
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        //calculate how much force we need to reach our desired jump height in unity meters
        float jumpForce = Mathf.Sqrt(jumpHeight * (-jumpDecay) * -2) * rb.mass;

        //add our jump force as an impluse force as we activate this script
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);

        pControl.remainingJumps -= 1;

        //set animation to jump
        //animName = "Player_Jump";

        //set placeholder animation to jump
        animName = "Idle";


    }

    public override void ExitState()
    {
        jumpTime = 0;

        //if we touch the ground make us grounded
        if (pControl.PlayerModel != null)
        {
            //rotate our player 360 degrees over the attack duration
            pControl.PlayerModel.transform.localRotation = startRotation;
        }

    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        if (rb.linearVelocity.y <= 0 && jumpTime > 0.1f)
        {
            //if our jump is no longer providing upwards force transition to falling
            return PlayerStateMachine.PlayerStates.spinFall;
        }

        if (pControl.Grounded && jumpTime > 0.4f)
        {
            

            return PlayerStateMachine.PlayerStates.grounded;
        }

        if (pControl.Grounded)
        {
            //a player is detected as ground below this player
            if (pControl.GroundObject != null)
            {
                if (pControl.GroundObject.CompareTag("Player"))
                {

                    pControl.GroundObject.GetComponent<Rigidbody>().AddForce(pControl.HeadSquishForce * Vector3.down, ForceMode.Impulse);
                    return PlayerStateMachine.PlayerStates.headBounce;
                }
            }

        }
        return stateKey;
    }

    public override void UpdateState()
    {
        //add up the time our jump has been active
        jumpTime += Time.deltaTime;

        ChangeGravity(pControl.JumpReleaseFactor, pControl.JumpPeakFactor);

        //pControl.PlayerModel.transform.rotation = startRotation * Quaternion.AngleAxis((Time.deltaTime / 0.15f) * 360f, Vector3.up);
        pControl.PlayerModel.transform.Rotate(Vector3.up, Time.deltaTime * 1500);
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

    public override void PhysicsUpdate()
    {

        //decay our upwards velocity by our decay rate, a faster decay rate means a faster jump upwards
        rb.AddForce((-jumpDecay * gravityFactor) * Vector3.up);
        
       
    }

}
