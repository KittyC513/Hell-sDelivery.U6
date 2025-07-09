using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeHangState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private Rigidbody rb;

    private Vector3 objStartPos;
    private bool fullySnapped = false;

    
    public PlayerLedgeHangState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
        
    }

    public override void EnterState()
    {
        rb = pControl.RB;
        fullySnapped = false;
        objStartPos = pControl.LastLedgeObj.transform.position;

        //pControl.transform.SetParent(pControl.LastLedgeObj.transform);

        //freeze player movement to stick to the ledge
        pControl.SetFreezeState(true, this.ToString());

        //set animation to jump
        animName = "Player_LedgeHang";
    }

    public override void ExitState()
    {
       // pControl.transform.SetParent(pControl.OriginalTransform);
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

        //target position + the change in position since the target position
        Vector3 changeInPos = objStartPos - pControl.LastLedgeObj.transform.position;

        targetPos = targetPos - changeInPos;

        if (Vector3.Distance(pControl.transform.position, targetPos) < 0.5f)
        {
            fullySnapped = true;
        }

        if (!fullySnapped)
        {
            //lerp to the target hang position 
            pControl.transform.position = Vector3.Lerp(pControl.transform.position, targetPos, 25 * Time.deltaTime);
        }   
        else
        {
            //lerp to the target hang position 
            pControl.transform.position = targetPos;
        }
  
    }

}
