using UnityEngine;
using UnityEngine.AI;

public class HellHoundWander : BaseState<HellHoundStateMachine.HoundStates>
{
    float maxWanderDistance;
    private float wanderTime;
    private float wanderTemp = 0;

    private NavMeshAgent navAgent;
    private HellHoundBase hellHoundBase;

    public HellHoundWander(HellHoundStateMachine.HoundStates key, HellHoundBase houndBase) : base(key)
    {
        hellHoundBase = houndBase;
        navAgent = hellHoundBase.NavAgent;
    }

    public override void EnterState()
    {
        Debug.Log("EnterState");
        maxWanderDistance = hellHoundBase.MaxWanderDistance;
        wanderTime = hellHoundBase.WanderTime;

        if (navAgent.isActiveAndEnabled) navAgent.SetDestination(GetNewPostion(maxWanderDistance, navAgent));
    }

    public override void ExitState()
    {
        
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        if (hellHoundBase.DetectPlayer(hellHoundBase.PlayerDetectionRadius, hellHoundBase.VisionConeAngle) != null)
        {
            return HellHoundStateMachine.HoundStates.chase;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        if (wanderTemp < wanderTime)
        {
            wanderTemp += Time.deltaTime;
        }
        else
        {
            Vector3 targetPos = GetNewPostion(maxWanderDistance, navAgent);

            if (targetPos.y - navAgent.transform.position.y < 0.2f)
            {
                navAgent.SetDestination(targetPos);
                wanderTemp = 0;
            }
        }

    }

    private Vector3 GetNewPostion(float radius, NavMeshAgent agent)
    {
        //every frame this function is called it will find a new position around a specified radius

        //get a random position inside a sphere around max wander distance
        Vector3 randomPos = Random.insideUnitSphere * radius;

        //make the position relative to the target
        randomPos += agent.transform.position;
        NavMeshHit meshHit;
        //get a position on the nav mesh using the random position
        NavMesh.SamplePosition(randomPos, out meshHit, radius + 20, NavMesh.AllAreas);
        //Debug.Log(meshHit.position);
        //return random nav mesh position
        return meshHit.position;
    }
}
