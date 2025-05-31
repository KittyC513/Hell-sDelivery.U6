using UnityEngine;

public class PlayerHeadBounce : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;

    private float jumpHeight;
    private float jumpDecay;

    private float jumpTime = 0;
    private float maxJumpTime = 1.7f;

    private float gravityFactor = 1;

    private PlayerController otherPlayer;

    private Rigidbody rb;

    public PlayerHeadBounce(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
       
    }

    public override void EnterState()
    {
        //reset the jump timer
        jumpTime = 0;

        if (jumpHeight == 0) jumpHeight = pControl.JumpHeight;
        if (jumpDecay == 0) jumpDecay = pControl.JumpDecayRate;

        //setup the rigidbody
        if (rb == null) rb = pControl.RB;

        //make sure our y velocity does not affect our current jump by setting it to 0
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        //set the gravity scale
        if (gravityFactor != pControl.gravityScale) gravityFactor = pControl.gravityScale;

        //calculate how much force we need to reach our desired jump height in unity meters
        float jumpForce = Mathf.Sqrt(jumpHeight * (-jumpDecay) * -2) * rb.mass;

        //if the player still has both jumps somehow, make sure they only have a double jump left
        if (pControl.remainingJumps > 1) pControl.remainingJumps = 1;

        //add upwards force to the player's rigidbody
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
    }

    public override void ExitState()
    {
        
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        if (rb.linearVelocity.y <= 0 && jumpTime > 0.1f)
        {
            //if our jump is no longer providing upwards force transition to falling
            return PlayerStateMachine.PlayerStates.airborne;
        }

        if (pControl.Grounded && jumpTime > 0.4f)
        {
            //if we touch the ground make us grounded
            return PlayerStateMachine.PlayerStates.grounded;
        }

        if (pControl.remainingJumps > 0 && pControl.DetectJumpInput() && jumpTime > 0.6f)
        {
            return PlayerStateMachine.PlayerStates.doubleJump;
        }

        return stateKey;
    }

    public override void UpdateState()
    {
        //add up the time our jump has been active
        jumpTime += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        //decay our upwards velocity by our decay rate, a faster decay rate means a faster jump upwards
        rb.AddForce((-jumpDecay * gravityFactor) * Vector3.up);
    }
}
