using UnityEngine;

public class HellHoundCooldown : BaseState<HellHoundStateMachine.HoundStates>
{
    private float cooldownTime;
    private float cooldownTemp;

    private HellHoundBase hellHoundBase;

    public HellHoundCooldown(HellHoundStateMachine.HoundStates key, HellHoundBase houndBase) : base(key)
    {
        hellHoundBase = houndBase;
        cooldownTime = hellHoundBase.CooldownTime;
    }

    public override void EnterState()
    {
        cooldownTemp = 0;
        hellHoundBase.NavAgent.updatePosition = false;
    }

    public override void ExitState()
    {
        hellHoundBase.NavAgent.Warp(hellHoundBase.NavAgent.transform.position);
        hellHoundBase.NavAgent.updatePosition = true;
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        if (cooldownTemp >= cooldownTime)
        {
            return HellHoundStateMachine.HoundStates.wander;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        cooldownTemp += Time.deltaTime;
    }
}
