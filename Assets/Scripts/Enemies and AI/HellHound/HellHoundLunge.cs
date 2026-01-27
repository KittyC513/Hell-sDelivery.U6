using UnityEngine;

public class HellHoundLunge : BaseState<HellHoundStateMachine.HoundStates>
{
    private HellHoundBase hellHoundBase;

    private float timer = 0;

    private float lockTime = 1;
    private float attackWindup = 1.5f;

    private bool lunged = false;
    

    private Vector3 targetPos;
    //have a certain amount of time where im locking onto the enemy
    //stop locking and start another delay to wait until attacking
    //disable navmesh and launch the enemy towards the player
    //launch at an arc towards the player (apply force towards player and upwards)

    public HellHoundLunge(HellHoundStateMachine.HoundStates key, HellHoundBase houndBase) : base(key)
    {
        hellHoundBase = houndBase;
    }

    public override void EnterState(HellHoundStateMachine.HoundStates lastState)
    {
        lockTime = hellHoundBase.lungeLockTime;
        attackWindup = hellHoundBase.lungeWindup;
        lunged = false;
        timer = 0;
    }

    public override void ExitState()
    {
        hellHoundBase.ToggleAttackHitbox(false);
    }

    public override HellHoundStateMachine.HoundStates GetNextState()
    {
        if (lunged && hellHoundBase.NavAgent.updatePosition && hellHoundBase.Grounded && timer >= lockTime + attackWindup + 0.2f)
        {
            hellHoundBase.shouldRotate = true;
            return HellHoundStateMachine.HoundStates.cooldown;
        }

        if (hellHoundBase.TakeHit)
        {
            return HellHoundStateMachine.HoundStates.takeHit;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        if (timer >= lockTime + attackWindup && !lunged)
        {

            //ready to lunge
            Lunge();
        }
        else if (timer >= lockTime)
        {
            //stop updating lock on time and get ready to lunge
        }
        else
        {
            //locking onto the player
            targetPos = hellHoundBase.TargetPlayer.transform.position;
        }

        if (hellHoundBase.Grounded && timer >= lockTime + attackWindup + 0.2f && lunged)
        {
            
            hellHoundBase.ToggleAttackHitbox(false);
        }
    }

    public override void PhysicsUpdate()
    {
        hellHoundBase.RotateTowards((hellHoundBase.TargetPlayer.transform.position - hellHoundBase.transform.position).normalized, 30);
    }
    public void Lunge()
    {
        hellHoundBase.shouldRotate = false;
        animName = "Pounce Attack In Place";
        Vector3 force;
        Vector3 directionToPlayer = hellHoundBase.TargetPlayer.transform.position - hellHoundBase.transform.position;
        directionToPlayer = directionToPlayer.normalized;
        force = new Vector3(directionToPlayer.x * 18, 7, directionToPlayer.z * 18);
        hellHoundBase.StartKnockback(force);
        lunged = true;
        hellHoundBase.ToggleAttackHitbox(true);
    }
}
