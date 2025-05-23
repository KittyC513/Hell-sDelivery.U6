using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ThiefSteal : BaseState<ThiefStateMachine.ThiefStates>
{
    private ThiefBase tBase;

    private float stealDelay;
    private float stealTime;

    private NavMeshAgent navAgent;
    private Rigidbody rb;

    private bool attackComplete = false;
    private bool didSteal = false;

    public ThiefSteal(ThiefStateMachine.ThiefStates key, ThiefBase thiefBase) : base(key)
    {
        //setup variables
        tBase = thiefBase;
        navAgent = tBase.NavAgent;
        stealDelay = tBase.StealDelay;
        stealTime = tBase.StealTime;
        rb = tBase.RB;
    }

    public override void EnterState()
    {
        tBase.StartCoroutine(AttackSequence());
    }

    public override void ExitState()
    {
        //if somehow another attack sequence is running stop it
        tBase.StopCoroutine(AttackSequence());
        //make sure rotation is reset
        tBase.shouldRotate = true;
        //let the navmesh know where it should be
        navAgent.Warp(tBase.transform.position);
        //update navmeshagent
        navAgent.updatePosition = true;
        //reset rigidbody just in case
        rb.linearVelocity = Vector3.zero;
        //reset attack complete for the next steal
        attackComplete = false;
    }

    public override ThiefStateMachine.ThiefStates GetNextState()
    {
        //if the stealing is complete run away regardless of if the thief actually got the package or not
        if (attackComplete)
        {
            return ThiefStateMachine.ThiefStates.run;
        }

        return stateKey;

    }

    public override void UpdateState()
    {
        
    }

    private IEnumerator AttackSequence()
    {
        //stop moving
        navAgent.updatePosition = false;
        rb.linearVelocity = Vector3.zero;

        //wait for a delay
        yield return new WaitForSeconds(stealDelay);
        //stop rotating
        tBase.shouldRotate = false;
        yield return new WaitForSeconds(stealDelay / 4);
        //check for player in range, if in range steal package
        if (tBase.DetectPlayer(tBase.StealRange, 45))
        {
            didSteal = true;
        }
        //steal time is how long the area is checked for a steal target
        yield return new WaitForSeconds(stealTime);

        yield return null;
        tBase.shouldRotate = true;
        attackComplete = true;
    }
}
