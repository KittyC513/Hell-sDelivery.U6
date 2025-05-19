using System.Collections;
using UnityEngine;


public class HellHoundAttack : BaseState<HellHoundStateMachine.HoundStates>
{
    private HellHoundStateMachine hellHoundController;
    private float attackDelayTime;
    private float attackDuration;

    private bool attackComplete = false;   
    public HellHoundAttack(HellHoundStateMachine.HoundStates key, HellHoundStateMachine houndController) : base(key)
    {
        hellHoundController = houndController;
        attackDelayTime = hellHoundController.AttackDelayTime;
        attackDuration = hellHoundController.AttackDuration;
    }

    public override void EnterState()
    {
        attackComplete = false;
        hellHoundController.StartCoroutine(AttackSequence());
    }

    public override void ExitState()
    {
        hellHoundController.StopCoroutine(AttackSequence());
        hellHoundController.NavAgent.updatePosition = true;
        hellHoundController.RB.linearVelocity = Vector3.zero;
        hellHoundController.shouldRotate = true;
        attackComplete = false;
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        //when the attack ends or the enemy takes a hit stop attacking
        if (attackComplete || hellHoundController.AddKnockback)
        {
            return HellHoundStateMachine.HoundStates.cooldown;
        }

        return stateKey;
    }

    public override void UpdateState()
    {
        
    }

    private IEnumerator AttackSequence()
    {
        //stop moving
        hellHoundController.NavAgent.updatePosition = false;
        hellHoundController.RB.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(attackDelayTime);
        hellHoundController.shouldRotate = false;
        yield return new WaitForSeconds(attackDelayTime / 4);
        //activate hitbox
        hellHoundController.ToggleAttackHitbox(true);
        yield return new WaitForSeconds(attackDuration);
        //disable hitbox
        hellHoundController.ToggleAttackHitbox(false);
        yield return null;
        //transition to cooldown
        hellHoundController.shouldRotate = true;
        attackComplete = true;
    }
}
