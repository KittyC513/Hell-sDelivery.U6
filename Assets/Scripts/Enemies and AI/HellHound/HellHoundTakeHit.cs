using UnityEngine;

public class HellHoundTakeHit : BaseState<HellHoundStateMachine.HoundStates>
{
    private float hitTime = 0.5f;
    private float timeTemp = 0;
    public HellHoundTakeHit(HellHoundStateMachine.HoundStates key) : base(key)
    {

    }

    public override void EnterState()
    {
        animName = "Take Damage";
    }

    public override void ExitState()
    {
        timeTemp = 0;
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        if (timeTemp >= hitTime)
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
