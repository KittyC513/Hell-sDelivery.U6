using UnityEngine;
using UnityEngine.AI;

public class ThiefRun : BaseState<ThiefStateMachine.ThiefStates>
{
    private ThiefBase tBase;
    private bool hasPackage = false;
    private float runAwayDist;

    private NavMeshAgent navAgent;
    public ThiefRun(ThiefStateMachine.ThiefStates key, ThiefBase thiefBase) : base(key)
    {
        tBase = thiefBase;
        hasPackage = tBase.hasPackage;
        navAgent = tBase.NavAgent;
    }

    public override void EnterState()
    {
        runAwayDist = tBase.PlayerDetectionRadius + 3;
    }

    public override void ExitState()
    {
        
    }

    public override ThiefStateMachine.ThiefStates GetNextState()
    {
        if (!tBase.DetectPlayer(runAwayDist, 360))
        {
            return ThiefStateMachine.ThiefStates.idle;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
    }

    public override void PhysicsUpdate()
    {

        if (!hasPackage)
        {
            Vector3 dir = (tBase.transform.position - tBase.TargetPlayer.transform.position).normalized;
            navAgent.SetDestination(dir * runAwayDist);
        }
    }
}
