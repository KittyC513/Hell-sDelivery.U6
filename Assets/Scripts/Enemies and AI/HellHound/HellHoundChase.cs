using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;

public class HellHoundChase : BaseState<HellHoundStateMachine.HoundStates>
{
    private HellHoundBase hellHoundBase;
    private NavMeshAgent navAgent;

    private float chaseTimeout = 1;
    private float chaseTemp = 0;
    private float chaseRange;

    private Rigidbody rb;


    public HellHoundChase(HellHoundStateMachine.HoundStates key, HellHoundBase houndBase) : base(key)
    {
        hellHoundBase = houndBase;
        navAgent = hellHoundBase.NavAgent;
        rb = hellHoundBase.RB;
    }

    public override void EnterState()
    {
        chaseRange = hellHoundBase.PlayerDetectionRadius;

        if (!hellHoundBase.AddKnockback) hellHoundBase.StartKnockback(Vector3.up * 5);
    }

    public override void ExitState()
    {
        
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        //if the distance between the player and agent is bigger than a certain range, count time until the connection between them times out
        if (Vector3.Distance(navAgent.transform.position, hellHoundBase.TargetPlayer.transform.position) > chaseRange)
        {
            chaseTemp += Time.deltaTime;

            if (chaseTemp >= chaseTimeout)
            {
                //reset to wander player is no longer in range
                hellHoundBase.ClearPlayer();
                return HellHoundStateMachine.HoundStates.wander;
            }
        }
        else
        {
            chaseTemp = 0;
        }

        if (Vector3.Distance(navAgent.transform.position, hellHoundBase.TargetPlayer.transform.position) < hellHoundBase.AttackDetectionRange)
        {
            return HellHoundStateMachine.HoundStates.attack;
        }

        return stateKey;
    }

    public override void UpdateState()
    {
        if (navAgent.enabled) ChasePlayer();

    }

    private void ChasePlayer()
    {
        navAgent.destination = hellHoundBase.TargetPlayer.transform.position;
    }
}
