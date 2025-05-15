using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAirborneState : BaseState<PlayerStateMachine.PlayerStates>
{
    //reference to our player controller
    private PlayerController pControl;
    private Vector3 goalVelocityChange;
    private Rigidbody rb;

    private float ledgeGrabHorizontalRange;
    private float ledgeGrabUpwardsRange;
    private LayerMask ledgeGrabMask;

    private float playerHitboxHeight;

    private bool ledgeDetected = false;



    public PlayerAirborneState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
        ledgeGrabHorizontalRange = pControl.LedgeGrabHorizontalRange;
        ledgeGrabUpwardsRange = pControl.LedgeGrabUpwardsRange;
        ledgeGrabMask = pControl.LedgeGrabMask;
    }

    public override void EnterState()
    {
        ledgeDetected = false;
        rb = pControl.RB;
        playerHitboxHeight = pControl.PlayerHitboxHeight;
        goalVelocityChange = Vector3.zero;
    }

    public override void UpdateState()
    {
        

    }

    public override void PhysicsUpdate()
    {
        if (!pControl.Grounded)
        {
            CalculateMovement(rb);
            ledgeDetected = DetectLedge();
        }
    }

    private void CalculateMovement(Rigidbody rb)
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetDir = Vector3.down;
        Vector3 xzVel = new Vector3(currentVel.x, 0, currentVel.z);

        //this is the speed we are trying to reach / our maximum speed with a direction provided by a camera dependant input
        Vector3 targetVelocity = targetDir * pControl.MaxFallSpeed;
       
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

    public override void ExitState()
    {
        ledgeDetected = false;
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        if (pControl.Grounded)
        {
            return PlayerStateMachine.PlayerStates.grounded;
        }

        if (pControl.CanCoyote && pControl.DetectJumpInput())
        {
            //we can do a regular jump if we are still in coyoteTime
            return PlayerStateMachine.PlayerStates.jump;
        }
        else if (pControl.remainingJumps > 0 && pControl.DetectJumpInput())
        {
            //otherwise we will double jump
            return PlayerStateMachine.PlayerStates.doubleJump;
        }

        if (ledgeDetected)
        {
            return PlayerStateMachine.PlayerStates.ledgeHang;
        }

        //TEMPORARY
        if (pControl.DetectAttackInput() && pControl.CheckCanAttack())
        {
            return PlayerStateMachine.PlayerStates.attack;
        }

        return stateKey;
        
        
    }

    private bool DetectLedge()
    {
        Vector3 direction = pControl.transform.TransformDirection(Vector3.forward);

        //where our downward pointing ray starts from (above the player and in front of the player)
        Vector3 downRayPos = pControl.transform.position + (new Vector3(ledgeGrabHorizontalRange * direction.x, ledgeGrabUpwardsRange, ledgeGrabHorizontalRange * direction.z));

        //where our double check ray shoots from, the same y position as our downward ray and starting inside the player
        Vector3 topForwardPos = pControl.transform.position + (new Vector3(0, ledgeGrabUpwardsRange, 0));

        //this spherecast shoots our forwards to check if any wall is there, this should help get rid of any jank with small gaps between colliders
        //if there is no collider detected we can look for the rest of the ledge hang
        if (!Physics.SphereCast(topForwardPos, 0.05f, direction, out RaycastHit notNeeded, ledgeGrabHorizontalRange - 0.05f, ledgeGrabMask))
        {
            //shoots a ray downwards to detect ground, if ground is detected that means a ledge is in front of the player
            if (Physics.Raycast(downRayPos, Vector3.down, out RaycastHit hit, pControl.LedgeGrabDownwardsRange, ledgeGrabMask))
            {
                Vector3 forwardRayPos = new Vector3(pControl.transform.position.x, hit.point.y - 0.01f, pControl.transform.position.z);
                //how high on our player controller we want to hang off the ledge
                float targetPlayerDist = (playerHitboxHeight / 2) - pControl.YHangOffset;

                //this ray shoots forward to find the wall that connects to the ledge we are hanging from
                if (Physics.Raycast(forwardRayPos, direction, out RaycastHit forwardHit, ledgeGrabHorizontalRange + 0.5f, ledgeGrabMask))
                {
                    //the target hanging position which is where we hit the ledge offset by where we want to be hanging
                    float targetYPos = hit.point.y - targetPlayerDist;

                    //this is how far away from the wall we want to be while hanging
                    Vector3 targetXZPos = forwardHit.point - direction * pControl.XZHangOffset;
                   
                    //set our position variables to be used by the ledge hang script
                    pControl.SetLedgeSnapVariables(targetXZPos, targetYPos);
                    return true;
                }
            }
        }

        return false;
    }
}
