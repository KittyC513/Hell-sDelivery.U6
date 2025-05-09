using UnityEngine;

public class PlayerRagdollState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Vector3 goalVelocityChange;

    public PlayerRagdollState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
    }

    public override void EnterState()
    {
        //disable player movement
        pControl.SetFreezeState(true, this.ToString());
        EnterRagdoll();
        //get a random direction to add some torque 
        Vector3 randomDirection = new Vector3(Random.Range(0.05f, 1.01f), Random.Range(0.05f, 1.01f), Random.Range(0.05f, 1.01f));

        //get an upwards force
        Vector3 forceDirection = new Vector3(0, 50, 0);

        //apply force and torque to spin and make the player look funny
        pControl.RB.AddForce(forceDirection, ForceMode.Impulse);
        pControl.RB.AddTorque(randomDirection * 0.2f, ForceMode.Impulse);


        goalVelocityChange = Vector3.zero;

    }

    public override void ExitState()
    {
        pControl.SetFreezeState(false, this.ToString());
        ExitRagdoll();
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        if (pControl.DetectJumpInput())
        {
            return PlayerStateMachine.PlayerStates.jump;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        
    }

    public override void PhysicsUpdate()
    {
        CalculateMovement(pControl.RB); 
    }

    private void EnterRagdoll()
    {
        pControl.RB.constraints = RigidbodyConstraints.None;
    }

    private void ExitRagdoll()
    {
        pControl.RB.constraints = RigidbodyConstraints.FreezeRotation;

        pControl.transform.rotation = Quaternion.identity;
    }

    private void CalculateMovement(Rigidbody rb)
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetDir = Vector3.down;
        Vector3 xzVel = new Vector3(currentVel.x, 0, currentVel.z);

        //this is the speed we are trying to reach / our maximum speed with a direction provided by a camera dependant input
        Vector3 targetVelocity = targetDir * (pControl.MaxFallSpeed);

        //how much we will change our velocity next step with smoothing by vector3.movetowards
        goalVelocityChange = Vector3.MoveTowards(goalVelocityChange, targetVelocity + xzVel, pControl.FallAccel * 0.02f);

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
