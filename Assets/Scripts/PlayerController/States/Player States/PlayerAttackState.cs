using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class PlayerAttackState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private PlayerAttackControl aControl;
    
    private float attackTime = 0.5f;
    private float attackTemp = 0;

    private float maximumVerticalSpeed;
    private float stallVelocity;

    private Quaternion startRotation;

    private Vector3 goalVelocityChange;

    //spin jumping variables
    private Vector3 vel;
    private Vector3 lastFramePos;
    private Rigidbody rb;

    public PlayerAttackState(PlayerStateMachine.PlayerStates key, PlayerController controller, PlayerAttackControl attackController) : base(key)
    {
        pControl = controller;
        aControl = attackController;
    }

    public override void EnterState(PlayerStateMachine.PlayerStates lastState)
    {
        //set animation
        //animName = "Player_Attack";

        //set placeholder animation
        animName = "Bite Attack";

        //set our total attack time 
        attackTime = aControl.AttackTime;

        //reset attack time
        attackTemp = 0;

        if (pControl.RB != null)
            rb = pControl.RB;

        if (pControl.PlayerModel != null)
        {
            //set the starting rotation to the rotation of the player right as this script starts
            startRotation = pControl.PlayerModel.transform.localRotation;
        }


        //don't let the player controller control rotation 
        //pControl.FreezeRotation(true, this.ToString());

        //reset goal velocity change for the freezing of y position
        goalVelocityChange = Vector3.zero;

        //reset the hitbox timer / trigger the hitbox
        aControl.ResetHitboxTime();

        stallVelocity = aControl.StallVelocity;
        maximumVerticalSpeed = aControl.MaximumVerticalSpeed;
      
    }

    public override void ExitState()
    {
        //give rotation control back to the player controller
        //pControl.FreezeRotation(false, this.ToString());
        if (pControl.PlayerModel != null)
        {
            //rotate our player 360 degrees over the attack duration
            pControl.PlayerModel.transform.localRotation = startRotation;
        }
        
        //reset the attack cooldown
        aControl.ResetAttackCooldown();
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        if (attackTemp >= attackTime && pControl.Grounded)
        {
            return PlayerStateMachine.PlayerStates.grounded;
        }
        else if (attackTemp >= attackTime && !pControl.Grounded)
        {
            return PlayerStateMachine.PlayerStates.airborne;
        }

        if (pControl.DetectJumpInput() && pControl.Grounded)
        {
            //if the player jumps off a moving platform apply their movement + the platforms movement to the player to add inertia 
            if (pControl.GroundObject.CompareTag("MovingPlat"))
            {
                //if the platform is not moving very much don't add force to the player
                if (vel.magnitude - rb.linearVelocity.magnitude > 0.9)
                {
                    //soften the velocity by removing some of the rigidbody movement
                    Vector3 softenedVel = vel - (rb.linearVelocity / 3);

                    //clamp the value to avoid some crazy niche scenarios where the velocity is crazy high for a frame
                    softenedVel = Vector3.ClampMagnitude(softenedVel, 60);

                    rb.AddForce(softenedVel, ForceMode.Impulse);
                }
            }
            return PlayerStateMachine.PlayerStates.spinJump;
        }

        return stateKey;
    }

    public override void UpdateState()
    {
        //add to attack timer
        if (attackTemp < attackTime) attackTemp += Time.deltaTime;

        //clamp attack temp to attackTime
        attackTemp = Mathf.Clamp(attackTemp, 0, attackTime);

        if (pControl.PlayerModel != null)
        {
            //rotate our player 360 degrees over the attack duration
            pControl.PlayerModel.transform.rotation = startRotation * Quaternion.AngleAxis((attackTemp / attackTime) * 360f, Vector3.up);
        }
     
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

    public override void PhysicsUpdate()
    {
       

        if (pControl.Grounded)
        {
            //if grounded and attacking add a speed boost
            pControl.RB.AddForce(pControl.ReadInputs() * aControl.AttackSpeedBoost);
            //move y velocity towards 0
            CalculateMovement(pControl.RB, Vector3.zero, 100, 0);
        }
        else
        {
            //move y velocity towards 0
            //CalculateMovement(pControl.RB, Vector3.up, stallVelocity, maximumVerticalSpeed);
            pControl.RB.linearVelocity = Vector3.MoveTowards(pControl.RB.linearVelocity, new Vector3(pControl.RB.linearVelocity.x, 0, pControl.RB.linearVelocity.z), stallVelocity);
        }
    }

    private void CalculateMovement(Rigidbody rb, Vector3 dir, float accel, float targetSpeed)
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetDir = dir;
        Vector3 xzVel = new Vector3(currentVel.x, 0, currentVel.z);

        //this is the speed we are trying to reach / our maximum speed with a direction provided by a camera dependant input
        Vector3 targetVelocity = targetDir * targetSpeed;

        //how much we will change our velocity next step with smoothing by vector3.movetowards
        goalVelocityChange = Vector3.MoveTowards(goalVelocityChange, targetVelocity + xzVel, accel * 0.02f);

        //the amount of velocity change needed to reach our maximum velocity
        Vector3 velocityChange = (goalVelocityChange - currentVel) / 0.02f;

        //maxAccelStep limits how much our velocity can change per step
        Vector3.ClampMagnitude(velocityChange, pControl.MaxFallAccelStep);

        //make sure we are only adding force in the Y value
        velocityChange = new Vector3(0, velocityChange.y, 0);

        //apply our force to our velocity
        rb.AddForce(velocityChange * rb.mass);
        //Debug.Log(velocityChange);
    }

   

}
