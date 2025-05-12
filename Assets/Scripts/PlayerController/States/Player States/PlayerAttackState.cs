using UnityEngine;

public class PlayerAttackState : BaseState<PlayerStateMachine.PlayerStates>
{
    private PlayerController pControl;

    private float attackTime = 0.5f;
    private float attackTemp = 0;

    private Quaternion startRotation;

    public PlayerAttackState(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key)
    {
        pControl = controller;
    }

    public override void EnterState()
    {
        attackTime = pControl.AttackTime;

        //reset attack time
        attackTemp = 0;

        //set the starting rotation to the rotation of the player right as this script starts
        startRotation = pControl.transform.rotation;

        //don't let the player controller control rotation 
        pControl.FreezeRotation(true, this.ToString());
    }

    public override void ExitState()
    {
        //give rotation control back to the player controller
        pControl.FreezeRotation(false, this.ToString());

        //reset the attack cooldown
        pControl.ResetAttackCooldown();
    }

    public override PlayerStateMachine.PlayerStates GetNextState()
    {
        if (attackTemp >= attackTime && pControl.Grounded)
        {
            return PlayerStateMachine.PlayerStates.grounded;
        }
        else if (attackTemp >= attackTime && !pControl.Grounded)
        {
            return PlayerStateMachine.PlayerStates.airborne;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        //add to attack timer
        if (attackTemp < attackTime) attackTemp += Time.deltaTime;

        //clamp attack temp to attackTime
        attackTemp = Mathf.Clamp(attackTemp, 0, attackTime);

        //rotate our player 360 degrees over the attack duration
        pControl.transform.rotation = startRotation * Quaternion.AngleAxis((attackTemp / attackTime) * 360f, Vector3.up);
    }

}
