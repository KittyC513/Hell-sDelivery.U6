using UnityEngine;

public class HellHoundCooldown : BaseState<HellHoundStateMachine.HoundStates>
{
    private float cooldownTime;
    private float cooldownTemp;

    private HellHoundStateMachine hellHoundController;

    public HellHoundCooldown(HellHoundStateMachine.HoundStates key, HellHoundStateMachine houndController) : base(key)
    {
        cooldownTime = houndController.CooldownTime;
        hellHoundController = houndController;
    }

    public override void EnterState()
    {
        cooldownTemp = 0;
        hellHoundController.NavAgent.updatePosition = false;
    }

    public override void ExitState()
    {
        hellHoundController.NavAgent.Warp(hellHoundController.NavAgent.transform.position);
        hellHoundController.NavAgent.updatePosition = true;
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
