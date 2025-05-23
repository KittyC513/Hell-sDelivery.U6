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
        //set variables
        tBase = thiefBase;
        navAgent = tBase.NavAgent;
        sneakSpeed = tBase.SneakSpeed;
        stealRange = tBase.StealRange;
    }

    public override void EnterState()
    {
        //set speed to a slower value for sneaking
        navAgent.speed = sneakSpeed;
    }

    public override void ExitState()
    {
        //on exit make sure the speed is reset to default
        navAgent.speed = tBase.DefaultSpeed;
    }

    public override ThiefStateMachine.ThiefStates GetNextState()
    {
        //if the player is detected at a close range attempt to steal the package
        if (tBase.DetectPlayer(stealRange, 45) != null)
        {
            //transition to steal
            return ThiefStateMachine.ThiefStates.steal;
        }


        //if the player is outside of sneak range return to idle
        if (tBase.DetectPlayer(tBase.PlayerDetectionRadius, 360) == null)
        {
            return ThiefStateMachine.ThiefStates.idle;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        //go towards the player
        navAgent.SetDestination(tBase.TargetPlayer.transform.position);
    }
}
