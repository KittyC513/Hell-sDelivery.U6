using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;

public class HellHoundChase : BaseState<HellHoundStateMachine.HoundStates>
{
    private HellHoundBase hellHoundBase;
    private NavMeshAgent navAgent;

    private float chaseTimeout = 3;
    private float chaseTemp = 0;
    private float lungeTime = 3;
    private float lungeTemp = 0;
    private float chaseRange;
    private float lungeRange = 10;

    private Rigidbody rb;



    public HellHoundChase(HellHoundStateMachine.HoundStates key, HellHoundBase houndBase) : base(key)
    {
        hellHoundBase = houndBase;
        navAgent = hellHoundBase.NavAgent;
        rb = hellHoundBase.RB;
    }

    public override void EnterState(HellHoundStateMachine.HoundStates lastState)
    {
        chaseRange = hellHoundBase.PlayerDetectionRadius;
        animName = "Hellhound_Alert";
        hellHoundBase.sfxPlayer.PlaySoundEffect("ObjectBank1", "HellHoundBark");
        if (!hellHoundBase.AddKnockback) hellHoundBase.StartKnockback(Vector3.up * 5);
        navAgent.speed = hellHoundBase.RunSpeed;
        lungeTemp = 0;
        lungeRange = hellHoundBase.lungeRange;
        lungeTime = hellHoundBase.lungeTimer;
    }

    public override void ExitState()
    {

    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        //if the hound can't see the player count time until connection timeout
        if (hellHoundBase.DetectPlayer(hellHoundBase.PlayerDetectionRadius, hellHoundBase.VisionConeAngle) == null)
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

            //player is within lunge range
            if (Vector3.Distance(navAgent.transform.position, hellHoundBase.TargetPlayer.transform.position) < lungeRange)
            {
                lungeTemp += Time.deltaTime;

                if (lungeTemp >= lungeTime)
                {
                    return HellHoundStateMachine.HoundStates.lunge;
                }
            }

            if (Vector3.Distance(navAgent.transform.position, hellHoundBase.TargetPlayer.transform.position) < hellHoundBase.AttackDetectionRange)
            {
                if (!hellHoundBase.TargetPlayer.GetComponent<Bag>().isInvisible)
                    return HellHoundStateMachine.HoundStates.attack;
            }

        }

       
        if (hellHoundBase.TakeHit)
        {
            return HellHoundStateMachine.HoundStates.takeHit;
        }

        return stateKey;
    }

    public override void UpdateState()
    {
        Vector3 xySpeed = new Vector3(navAgent.velocity.x, 0, navAgent.velocity.z);
        //set placeholder animation
        hellHoundBase.animator.SetFloat("Speed", xySpeed.magnitude);
        hellHoundBase.animator.SetBool("Grounded", hellHoundBase.Grounded);
        if (navAgent.enabled) ChasePlayer();

    }

    private void ChasePlayer()
    {
        //if (hellHoundBase.TargetPlayer.GetComponent<Bag>().isInvisible)
        //{
        //    hellHoundBase.ClearPlayer();
        //    hellHoundBase.hellHoundStateMachine.OverrideState(HellHoundStateMachine.HoundStates.wander);
        //    return;
        //}

        navAgent.destination = hellHoundBase.TargetPlayer.transform.position;
    }
}
