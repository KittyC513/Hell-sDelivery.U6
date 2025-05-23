using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class ThiefIdle : BaseState<ThiefStateMachine.ThiefStates>
{
    private ThiefBase tBase;
    private NavMeshAgent navAgent;

    private float wanderTime;
    private float wanderTemp;
    private float maxWanderDistance;
    public ThiefIdle(ThiefStateMachine.ThiefStates key, ThiefBase thiefBase) : base(key)
    {
        //setup variables
        tBase = thiefBase;
        navAgent = tBase.NavAgent;
        maxWanderDistance = tBase.MaxWanderDistance;
        wanderTime = tBase.WanderTime;
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {
        
    }

    public override ThiefStateMachine.ThiefStates GetNextState()
    {
        //if the player is detected in a range around the thief start sneaking towards them
        if (tBase.DetectPlayer(tBase.PlayerDetectionRadius, 360))
        {
            return ThiefStateMachine.ThiefStates.sneak;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        //wander for x amount of time unitl choosing a new direction
        if (wanderTemp < wanderTime)
        {
            wanderTemp += Time.deltaTime;
        }
        else
        {
            Vector3 targetPos = GetNewPostion(maxWanderDistance, navAgent);

            //check if the new position is on the same vertical section of nav mesh otherwise choose a new position
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
