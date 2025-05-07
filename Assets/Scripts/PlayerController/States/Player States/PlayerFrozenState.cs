using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this state will be able to freeze the state machine as it has no natural exit and instead has to be exited manually from an override function
//this script will freeze the player's movement but leave the rigidbody able to have forces added to it
//this state is useful for other scripts that need to override the player's movement 
public class PlayerFrozenState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;

    public PlayerFrozenState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
    }

    public override void EnterState()
    {
        pControl.SetFreezeState(true, this.ToString());
    }

    public override void ExitState()
    {
        pControl.SetFreezeState(false, this.ToString());
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        return stateKey;
    }

    public override void UpdateState()
    {
        
    }
}
