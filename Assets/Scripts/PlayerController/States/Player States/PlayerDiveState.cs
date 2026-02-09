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
        pControl.canDive = false;
        rb = pControl.RB;
        pControl.rotationSpeed = pControl.diveRotationSpeed;
        forward = pControl.transform.forward;

        diveForwardForce = pControl.diveForwardForce;
        diveUpwardForce = pControl.diveUpwardForce;

        currentDiveForce = diveForwardForce;
        rb.AddForce((diveForwardForce * forward) + (diveUpwardForce * Vector3.up), ForceMode.Impulse);
        animName = "Dive";
        goalVelocityChange = Vector3.zero;
    }

    public override void ExitState()
    {
        pControl.rotationSpeed = pControl.startRotationSpeed;
        pControl.diveGrounded = false;
    }

    public override void UpdateState()
    {
        if (pControl.Grounded)
        {
            pControl.diveGrounded = true;
        }
        else
        {
            pControl.diveGrounded = false;
        }
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

            rb.AddForce((diveForwardForce * pControl.transform.forward), ForceMode.Impulse);
            return PlayerStateMachine.PlayerStates.jump;
        }
        else if (pControl.DetectJumpInput() && pControl.remainingJumps > 0)
        {
            return PlayerStateMachine.PlayerStates.doubleJump;
        }

        if (DetectLedge())
        {
            return PlayerStateMachine.PlayerStates.ledgeHang;
        }

         if (pControl.Grounded)
        {
            if (pControl.GroundObject.CompareTag("BouncePad"))
            {
                pControl.BouncePad();
                return PlayerStateMachine.PlayerStates.jump;
            }
            //a player is detected as ground below this player
            if (pControl.GroundObject.CompareTag("Player"))
            {
                pControl.GroundObject.GetComponent<Rigidbody>().AddForce(pControl.HeadSquishForce * Vector3.down, ForceMode.Impulse);
                return PlayerStateMachine.PlayerStates.headBounce;
            }
            //return PlayerStateMachine.PlayerStates.grounded;
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

     private bool DetectLedge()
    {
        Vector3 direction = pControl.transform.TransformDirection(Vector3.forward);

        //where our downward pointing ray starts from (above the player and in front of the player)
        Vector3 downRayPos = pControl.transform.position + (new Vector3(pControl.LedgeGrabHorizontalRange * direction.x, pControl.LedgeGrabUpwardsRange, pControl.LedgeGrabHorizontalRange * direction.z));

        //where our double check ray shoots from, the same y position as our downward ray and starting inside the player
        Vector3 topForwardPos = pControl.transform.position + (new Vector3(0, pControl.LedgeGrabUpwardsRange, 0));

        //this spherecast shoots our forwards to check if any wall is there, this should help get rid of any jank with small gaps between colliders
        //if there is no collider detected we can look for the rest of the ledge hang
        if (!Physics.SphereCast(topForwardPos, 0.05f, direction, out RaycastHit notNeeded, pControl.LedgeGrabHorizontalRange - 0.05f, pControl.LedgeGrabMask))
        {
            //shoots a ray downwards to detect ground, if ground is detected that means a ledge is in front of the player
            if (Physics.Raycast(downRayPos, Vector3.down, out RaycastHit hit, pControl.LedgeGrabDownwardsRange, pControl.LedgeGrabMask))
            {
                Vector3 forwardRayPos = new Vector3(pControl.transform.position.x, hit.point.y - 0.01f, pControl.transform.position.z);
                //how high on our player controller we want to hang off the ledge
                float targetPlayerDist = (pControl.PlayerHitboxHeight / 2) - pControl.YHangOffset;

                //this ray shoots forward to find the wall that connects to the ledge we are hanging from
                if (Physics.Raycast(forwardRayPos, direction, out RaycastHit forwardHit, pControl.LedgeGrabHorizontalRange + 0.5f, pControl.LedgeGrabMask))
                {
                    //the target hanging position which is where we hit the ledge offset by where we want to be hanging
                    float targetYPos = hit.point.y - targetPlayerDist;

                    //this is how far away from the wall we want to be while hanging
                    Vector3 targetXZPos = forwardHit.point - direction * pControl.XZHangOffset;
                   
                    //set our position variables to be used by the ledge hang script
                    pControl.SetLedgeSnapVariables(targetXZPos, targetYPos, hit.transform.gameObject);
                    return true;
                }
            }
        }

        return false;
    }
}
