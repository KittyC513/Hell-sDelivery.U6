using UnityEngine;

public class PlayerDeadState : BaseState<PlayerStateMachine.PlayerStates>
{
    public PlayerController playerController;
    public PlayerDeadState(PlayerStateMachine.PlayerStates key, PlayerController playerController) : base(key)
    {
        this.playerController = playerController;
    }

    public override void EnterState(PlayerStateMachine.PlayerStates lastState)
    {
        animName = "Die";
       

        if (playerController.playerNum == 1)
        {
             playerController.playerSfx.PlayShmonkDeath();
        }
        else
        {
             playerController.playerSfx.PlayShimnkDeath();
        }

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
