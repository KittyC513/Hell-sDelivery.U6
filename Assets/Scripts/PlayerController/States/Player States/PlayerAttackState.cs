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

    public PlayerAttackState(PlayerStateMachine.PlayerStates key, PlayerController controller, PlayerAttackControl attackController) : base(key)
    {
        pControl = controller;
        aControl = attackController;
    }

    public override void EnterState()
    {
        //set animation
        animName = "Player_Attack";

        //set our total attack time 
        attackTime = aControl.AttackTime;

        //reset attack time
        attackTemp = 0;

        //set the starting rotation to the rotation of the player right as this script starts
        startRotation = pControl.transform.rotation;

        //don't let the player controller control rotation 
        pControl.FreezeRotation(true, this.ToString());

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
        pControl.FreezeRotation(false, this.ToString());

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
        return stateKey;
    }

    public override void UpdateState()
    {
        //add to attack timer
        if (attackTemp < attackTime) attackTemp += Time.deltaTime;

        //clamp attack temp to attackTime
        attackTemp = Mathf.Clamp(attackTemp, 0, attackTime);

        //rotate our player 360 degrees over the attack duration
        pControl.transform.rotation = startRotation * Quaternion.AngleAxis((attackTemp / attackTime) * 360f, Vector3.up);
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
            CalculateMovement(pControl.RB, Vector3.up, stallVelocity, maximumVerticalSpeed);
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
