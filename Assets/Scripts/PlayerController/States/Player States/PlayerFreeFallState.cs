using System.Threading;
using UnityEngine;

public class PlayerFreeFallState : BaseState<PlayerStateMachine.PlayerStates>
{
    //the player enters free fall
    //they have limited control and can only jump out once the cooldown timer is up
    //they spin around like they got blasted away

    private PlayerController pControl;
    private Rigidbody rb;

    private float stunTime = 0.5f; 
    private float stateTime = 0; //how long this state has been active

    private float rotateTime = 0.01f;
    private float angleStep = 15;
    private float rotateTemp = 0;

    private Vector3 goalVelocityChange;

    public PlayerFreeFallState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
        rb = pControl.GetComponent<Rigidbody>();
    }

    public override void EnterState()
    {
        //reset variables
        stateTime = 0;
        goalVelocityChange = Vector3.zero;
        pControl.FreezeRotation(true, this.ToString());

        //set rotation to a multiple of angle step
        pControl.PlayerModel.transform.rotation = Quaternion.Euler(0, 0, angleStep);

    }

    public override void ExitState()
    {
        pControl.PlayerModel.transform.localRotation = Quaternion.identity;
        pControl.FreezeRotation(false, this.ToString());
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        //splat the player on the ground
        if (pControl.Grounded && stateTime >= stunTime)
        {
            //splat on the ground
            return PlayerStateMachine.PlayerStates.splat;
        }

        //let the player jump out after the stun time
        if (pControl.remainingJumps > 0 && pControl.DetectJumpInput() && stateTime >= stunTime)
        {
            //break out with a double jump
            return PlayerStateMachine.PlayerStates.doubleJump;
        }

        return stateKey;
    }

    public override void UpdateState()
    {
        stateTime += Time.deltaTime;
        rotateTemp += Time.deltaTime;

        //rotate the player at intervals 
        if (rotateTemp >= rotateTime)
        {
            Quaternion currentRot = pControl.PlayerModel.transform.rotation;
            rotateTemp = 0;
            pControl.PlayerModel.transform.rotation = currentRot * Quaternion.Euler(0, 0, angleStep);
        }
        
    }

    public override void PhysicsUpdate()
    {
        //apply downwards gravity to the player
        CalculateMovement(rb);
    }

    private void CalculateMovement(Rigidbody rb)
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetDir = Vector3.down;
        Vector3 xzVel = new Vector3(currentVel.x, 0, currentVel.z);

        //this is the speed we are trying to reach / our maximum speed with a direction provided by a camera dependant input
        Vector3 targetVelocity = targetDir * ((pControl.MaxFallSpeed * pControl.GravityScale) / 2);

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

    }

}
