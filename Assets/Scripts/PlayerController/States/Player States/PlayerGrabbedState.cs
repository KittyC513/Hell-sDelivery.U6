using UnityEngine;

public class PlayerGrabbedState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;
    private PlayerObjectController oControl;

    public PlayerGrabbedState(PlayerStateMachine.PlayerStates key, PlayerController controller, PlayerObjectController objectControl) : base(key)
    {
        pControl = controller;
        oControl = objectControl;
    }

    public override void EnterState(PlayerStateMachine.PlayerStates lastState)
    {
        pControl.SetFreezeState(true, this.ToString());
    }

    public override void ExitState()
    {
        oControl.canPickup = true;
        pControl.SetFreezeState(false, this.ToString());
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        return stateKey;
    }

    public override void UpdateState()
    {
        //setting this in update in case anything else tries to edit this value
        //we do not want the players picking up each other at the same time
        oControl.canPickup = false;
    }
}
