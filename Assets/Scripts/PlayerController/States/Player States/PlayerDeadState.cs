using UnityEngine;

public class PlayerDeadState : BaseState<PlayerStateMachine.PlayerStates>
{
    public PlayerController playerController;
    public PlayerDeadState(PlayerStateMachine.PlayerStates key, PlayerController playerController) : base(key)
    {
        this.playerController = playerController;
    }

    public override void EnterState()
    {
        animName = "Die";
        playerController.SetFreezeState(true, this.ToString());
    }

    public override void ExitState()
    {
        playerController.SetFreezeState(false, this.ToString());
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        return stateKey;
    }

    public override void UpdateState()
    {

    }

}
