using System.Collections;
using UnityEngine;


public class HellHoundAttack : BaseState<HellHoundStateMachine.HoundStates>
{
    private HellHoundBase hellHoundBase;
    private float attackDelayTime;
    private float attackDuration;

    private bool attackComplete = false;   
    public HellHoundAttack(HellHoundStateMachine.HoundStates key, HellHoundBase houndBase) : base(key)
    {
        hellHoundBase = houndBase;
        attackDelayTime = hellHoundBase.AttackDelayTime;
        attackDuration = hellHoundBase.AttackDuration;
    }

    public override void EnterState(HellHoundStateMachine.HoundStates lastState)
    {
        attackComplete = false;
        hellHoundBase.StartCoroutine(AttackSequence());

        //set placeholder animation
        animName = "Hellhound_Bite";
    }

    public override void ExitState()
    {
        hellHoundBase.StopCoroutine(AttackSequence());
        hellHoundBase.NavAgent.updatePosition = true;
        hellHoundBase.RB.linearVelocity = Vector3.zero;
        hellHoundBase.shouldRotate = true;
        attackComplete = false;
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        //when the attack ends or the enemy takes a hit stop attacking
        if (attackComplete)
        {
            return HellHoundStateMachine.HoundStates.cooldown;
        }

        if (hellHoundBase.TakeHit)
        {
            return HellHoundStateMachine.HoundStates.takeHit;
        }

        return stateKey;
    }

    public override void UpdateState()
    {
        
    }

    private IEnumerator AttackSequence()
    {
        //stop moving
        hellHoundBase.NavAgent.updatePosition = false;
        hellHoundBase.RB.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(attackDelayTime);
        hellHoundBase.shouldRotate = false;
        yield return new WaitForSeconds(attackDelayTime / 4);
        //activate hitbox
        if (!hellHoundBase.Dead) hellHoundBase.ToggleAttackHitbox(true);
        yield return new WaitForSeconds(attackDuration);
        //disable hitbox
        if (!hellHoundBase.Dead) hellHoundBase.ToggleAttackHitbox(false);
        yield return null;
        //transition to cooldown
        hellHoundBase.shouldRotate = true;
        attackComplete = true;
    }
}
