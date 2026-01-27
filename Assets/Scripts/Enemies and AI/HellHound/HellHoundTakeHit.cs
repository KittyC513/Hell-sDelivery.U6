using UnityEngine;

public class HellHoundTakeHit : BaseState<HellHoundStateMachine.HoundStates>
{
    private float hitTime = 0.5f;
    private float timeTemp = 0;
    private HellHoundBase hellHoundBase;
    public HellHoundTakeHit(HellHoundStateMachine.HoundStates key, HellHoundBase houndBase) : base(key)
    {
        hellHoundBase = houndBase;
    }

    public override void EnterState(HellHoundStateMachine.HoundStates lastState)
    {
        hellHoundBase.ToggleAttackHitbox(false);
        if (!hellHoundBase.Dead)
        {
            animName = "Take Damage";
        }
        else
        {
            animName = "Die";
        }
        
    }

    public override void ExitState()
    {
        timeTemp = 0;
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        if (timeTemp >= hitTime && hellHoundBase.NavAgent.enabled == true && !hellHoundBase.Dead)
        {
            return HellHoundStateMachine.HoundStates.wander;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        timeTemp += 1 * Time.deltaTime;
    }
}
