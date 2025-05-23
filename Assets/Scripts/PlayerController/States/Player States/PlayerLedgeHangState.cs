using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeHangState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Rigidbody rb;

    
    public PlayerLedgeHangState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
        
    }

    public override void EnterState()
    {
        rb = pControl.RB;
        
        //freeze player movement to stick to the ledge
        pControl.SetFreezeState(true, this.ToString());

        //set animation to jump
        animName = "Player_LedgeHang";
    }

    public override void ExitState()
    {
        pControl.SetFreezeState(false, this.ToString());
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        if (pControl.DetectJumpInput())
        {
            pControl.remainingJumps = pControl.MaxJumps;
            return PlayerStateMachine.PlayerStates.jump;
        }
        return stateKey;
    }

    public override void UpdateState()
    {

    }

    public override void PhysicsUpdate()
    {
        //get our target hang position from the player controller
        Vector3 targetPos = new Vector3(pControl.LastLedgeXZ.x, pControl.LastLedgeY, pControl.LastLedgeXZ.z);

        //lerp to the target hang position 
        pControl.transform.position = Vector3.Lerp(pControl.transform.position, targetPos, 15 * Time.deltaTime);
    }

}
