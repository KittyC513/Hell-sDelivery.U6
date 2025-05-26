using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;


public class NavmeshWander<State> : BaseState<State> where State : Enum
{
    float maxWanderDistance = 5;
    private float wanderTime = 5;
    private float wanderTemp = 0;

    private NavMeshAgent navAgent;
    private StateMachine hellHoundController;


    public NavmeshWander(State key, StateManager<State> stateMachine) : base(key)
    {
       
    }

    public override void EnterState()
    {
        navAgent.SetDestination(GetNewPostion(maxWanderDistance, navAgent));
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

    public override void PhysicsUpdate()
    {
        

    }

    private Vector3 GetNewPostion(float radius, NavMeshAgent agent)
    {
        //every frame this function is called it will find a new position around a specified radius

        //get a random position inside a sphere around max wander distance
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * radius;

        //make the position relative to the target
        randomPos += agent.transform.position;
        NavMeshHit meshHit;
        //get a position on the nav mesh using the random position
        NavMesh.SamplePosition(randomPos, out meshHit, radius + 20, NavMesh.AllAreas);
        Debug.Log(meshHit.position);
        //return random nav mesh position
        return meshHit.position;
    }

}
