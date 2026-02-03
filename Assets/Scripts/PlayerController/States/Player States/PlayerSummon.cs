using UnityEngine;

public class PlayerSummon : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;

    public PlayerSummon(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
    }

     public override void EnterState(PlayerStateMachine.PlayerStates lastState)
    {
        pControl.SetFreezeState(true, this.ToString());
        animName = "Summon";
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        return stateKey;
    }

    public override void UpdateState()
    {
     
    }
    public override void ExitState()
    {
        pControl.SetFreezeState(false, this.ToString());
    }
   
}
