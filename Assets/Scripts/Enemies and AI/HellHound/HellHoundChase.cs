using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;

public class HellHoundChase : BaseState<HellHoundStateMachine.HoundStates>
{
    private HellHoundStateMachine hellHoundController;
    private NavMeshAgent navAgent;

    private float chaseTimeout = 1;
    private float chaseTemp = 0;
    private float chaseRange;

    private Rigidbody rb;


    public HellHoundChase(HellHoundStateMachine.HoundStates key, HellHoundStateMachine stateMachine) : base(key)
    {
        hellHoundController = stateMachine;
        navAgent = stateMachine.NavAgent;
        rb = hellHoundController.RB;
    }

    public override void EnterState()
    {
        chaseRange = hellHoundController.PlayerDetectionRadius;

        if (!hellHoundController.AddKnockback) hellHoundController.StartKnockback(Vector3.up * 5);
    }

    public override void ExitState()
    {
        
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        //if the distance between the player and agent is bigger than a certain range, count time until the connection between them times out
        if (Vector3.Distance(navAgent.transform.position, hellHoundController.TargetPlayer.transform.position) > chaseRange)
        {
            chaseTemp += Time.deltaTime;

            if (chaseTemp >= chaseTimeout)
            {
                //reset to wander player is no longer in range
                hellHoundController.ClearPlayer();
                return HellHoundStateMachine.HoundStates.wander;
            }
        }
        else
        {
            chaseTemp = 0;
            return stateKey;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        if (navAgent.enabled) ChasePlayer();

    }

    private void ChasePlayer()
    {
        navAgent.destination = hellHoundController.TargetPlayer.transform.position;
    }
}
