using UnityEngine;

public class PlayerSplatState : BaseState<PlayerStateMachine.PlayerStates>
{
    //when the player lands from a freefall they face plant
    private PlayerController pControl;
    public PlayerSplatState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
    }

    public override void EnterState(PlayerStateMachine.PlayerStates lastState)
    {
        pControl.SetFreezeState(true, this.ToString());
        pControl.FreezeRotation(true, this.ToString());
        pControl.PlayerModel.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    public override void ExitState()
    {
        pControl.PlayerModel.transform.localRotation = Quaternion.identity;
        pControl.SetFreezeState(false, this.ToString());
        pControl.FreezeRotation(false, this.ToString());
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        //let thep layer jump off the ground
        if (pControl.DetectJumpInput())
        {
            return PlayerStateMachine.PlayerStates.jump;
        }

        //if they are knocked off the ground go back to free falling
        if (pControl.Grounded == false)
        {
            return PlayerStateMachine.PlayerStates.freeFall;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        pControl.RB.linearVelocity = Vector3.zero;
    }
}
