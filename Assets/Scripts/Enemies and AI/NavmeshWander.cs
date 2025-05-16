using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using static UnityEditorInternal.ReorderableList;


public class NavmeshWander<State> : BaseState<State> where State : Enum
{
    float maxWanderDistance = 5;
    private NavMeshAgent navAgent;

    public NavmeshWander(State key, NavMeshAgent agent, float maxWanderDist) : base(key)
    {
        maxWanderDistance = maxWanderDist;
        navAgent = agent;
    }

    public override void EnterState()
    {
        navAgent.SetDestination(GetNewPostion(maxWanderDistance));
    }

    public override void ExitState()
    {
        
    }

    public override State GetNextState()
    {
        return stateKey;
    }

    public override void UpdateState()
    {
        if (navAgent.remainingDistance == 0 && navAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            navAgent.SetDestination(GetNewPostion(maxWanderDistance));
            
        }
    }

    public override void PhysicsUpdate()
    {
        

    }

    private Vector3 GetNewPostion(float radius)
    {
        //every frame this function is called it will find a new position around a specified radius

        //get a random position inside a sphere around max wander distance
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * radius;

        //make the position relative to the target
        randomPos += navAgent.transform.position;
        NavMeshHit meshHit;
        //get a position on the nav mesh using the random position
        NavMesh.SamplePosition(randomPos, out meshHit, radius + 20, NavMesh.AllAreas);
        Debug.Log(meshHit.position);
        //return random nav mesh position
        return meshHit.position;
    }
}
