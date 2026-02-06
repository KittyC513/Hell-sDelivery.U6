using TMPro;
using UnityEngine;

public class PlayerDiveState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Rigidbody rb;
    private float diveForwardForce = 35;
    private float diveUpwardForce = 6;
    private Vector3 forward;

    private Vector3 vel;
    private Vector3 lastFramePos;
    private Vector3 goalVelocityChange;

    private float currentDiveForce;

    public PlayerDiveState(PlayerStateMachine.PlayerStates key, PlayerController playerController) : base(key)
    {
        pControl = playerController;
    }

    public override void EnterState(PlayerStateMachine.PlayerStates lastState)
    {
        rb = pControl.RB;
        pControl.rotationSpeed = pControl.diveRotationSpeed;
        forward = pControl.transform.forward;
        currentDiveForce = diveForwardForce;
        rb.AddForce((diveForwardForce * forward) + (diveUpwardForce * Vector3.up), ForceMode.Impulse);
        animName = "Dive";
        goalVelocityChange = Vector3.zero;
    }

    public override void ExitState()
    {
        pControl.rotationSpeed = pControl.startRotationSpeed;
    }

    public override void UpdateState()
    {

    }

    public override void PhysicsUpdate()
    {
        if (currentDiveForce >= 1)
        {
           //rb.AddForce(currentDiveForce * Time.fixedDeltaTime * pControl.transform.forward);
           currentDiveForce -= 1 * Time.fixedDeltaTime; 
        }
        
        CalculateMovement(rb);
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
    
        if (pControl.DetectJumpInput() && pControl.Grounded)
        {
            //reset our maximum jumps
            pControl.remainingJumps = pControl.MaxJumps;

            //if the player jumps off a moving platform apply their movement + the platforms movement to the player to add inertia 
            if (pControl.GroundObject.CompareTag("MovingPlat"))
            {
                //if the platform is not moving very much don't add force to the player
                if (vel.magnitude - rb.linearVelocity.magnitude > 0.9)
                {
                    //soften the velocity by removing some of the rigidbody movement
                    Vector3 softenedVel = vel - (rb.linearVelocity/3);

                    //clamp the value to avoid some crazy niche scenarios where the velocity is crazy high for a frame
                    softenedVel = Vector3.ClampMagnitude(softenedVel, 60);

                    rb.AddForce(softenedVel, ForceMode.Impulse);
                }
            }

            rb.AddForce((diveForwardForce / 2 * pControl.transform.forward), ForceMode.Impulse);
            return PlayerStateMachine.PlayerStates.jump;
        }
        else if (pControl.DetectJumpInput() && pControl.remainingJumps > 0)
        {
            return PlayerStateMachine.PlayerStates.doubleJump;
        }

        return stateKey;
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

    private void CalculateMovement(Rigidbody rb)
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetDir = Vector3.down;
        Vector3 xzVel = new Vector3(currentVel.x, 0, currentVel.z);

        //this is the speed we are trying to reach / our maximum speed with a direction provided by a camera dependant input
        Vector3 targetVelocity = targetDir * (pControl.MaxFallSpeed * pControl.GravityScale);
       
        //how much we will change our velocity next step with smoothing by vector3.movetowards
        goalVelocityChange = Vector3.MoveTowards(goalVelocityChange, targetVelocity + xzVel, (pControl.FallAccel * pControl.fallAccelScale) * 0.02f);
        
        //the amount of velocity change needed to reach our maximum velocity
        Vector3 velocityChange = (goalVelocityChange - currentVel) / 0.02f;

        //maxAccelStep limits how much our velocity can change per step
        velocityChange = Vector3.ClampMagnitude(velocityChange, pControl.MaxFallAccelStep);

        //make sure we are only adding force in the Y value
        velocityChange = new Vector3(0, velocityChange.y, 0);

        //apply our force to our velocity
        rb.AddForce(velocityChange * rb.mass);
        //Debug.Log(velocityChange);
        //downForce = currentVel.y;
        
    }
}
