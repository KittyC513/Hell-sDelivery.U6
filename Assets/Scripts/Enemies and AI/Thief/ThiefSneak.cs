using UnityEngine;
using UnityEngine.AI;

public class ThiefSneak : BaseState<ThiefStateMachine.ThiefStates>
{
    private ThiefBase tBase;

    private float sneakSpeed;
    private NavMeshAgent navAgent;
    private float stealRange = 1;

    public ThiefSneak(ThiefStateMachine.ThiefStates key, ThiefBase thiefBase) : base(key)
    {
        tBase = thiefBase;
        navAgent = tBase.NavAgent;
    }

    public override void EnterState()
    {
        navAgent.speed = sneakSpeed;
    }

    public override void ExitState()
    {
        
    }

    public override ThiefStateMachine.ThiefStates GetNextState()
    {
        //if the player is detected at a close range attempt to steal the package
        if (tBase.DetectPlayer(1, 45) != null)
        {
            //transition to steal
            return ThiefStateMachine.ThiefStates.steal;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        navAgent.SetDestination(tBase.TargetPlayer.transform.position);
    }
}
