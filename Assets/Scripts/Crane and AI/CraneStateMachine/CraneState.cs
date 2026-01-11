using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Drawing.Printing;

public class CraneState
{
    public enum STATE
    {
        IDEL,
        MOVINGTOPICKUP,
        PICKINGUP,
        MOVINGTODROPPINGOFF,
        DROPPINGOFF,
        EMERGENCYSTOP,
        ATTACK,
        PURSUE
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected CraneState nextState;
    protected NavMeshAgent agent;
    protected Vector3 pickupLocation;
    protected Vector3 dropoffLocation;
    protected bool armHasLoad = false;

    public float visRadius = 10f;
    public float visAngle = 30f;
    public float rotateSpeed = 5f;

    protected bool hasInitiatedAttack = false;
    

    public CraneState(GameObject _npc, NavMeshAgent _agent, Animator _anim)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        //player = _player;
        //pickupLocation = _pickupLocation;
        //dropoffLocation = _dropoffLocation;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { }
    public virtual void Exit() { }

    public CraneState Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    #region Chasing and Attacking player condition
    //check if crane can see player within vision radius and angle
    public bool CanSeePlayer()
    {
        Collider[] players = Physics.OverlapSphere(npc.transform.position, visRadius, 
            1 << LayerMask.NameToLayer("Player1") | 1<<LayerMask.NameToLayer("Player2"));

        if (players.Length == 0)
        {
            Debug.Log("Crane cannot see player: No players in vision radius.");
            return false;
        }
        else
        {
            Debug.Log("Crane found players in vision radius.");
            return true;
        }

    }

    //check if crane arm has load to attack player
    public bool CanAttackPlayer()
    {

        if (armHasLoad) 
            return true;
        else 
            return false;
    }
    #endregion
}

#region Idle
public class Idle : CraneState
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _pickupLocation, Vector3 _dropoffLocation)
        : base(_npc, _agent, _anim)
    {
        pickupLocation = _pickupLocation;
        Debug.Log("Pickup Location set to: " + pickupLocation);
        dropoffLocation = _dropoffLocation;
        Debug.Log("Dropoff Location set to: " + dropoffLocation);
        name = STATE.IDEL;
    }
    public override void Enter()
    {
        base.Enter();
        // set idle trigger
        //anim.SetTrigger("isIdle");
        Debug.Log("Crane is now Idle.");
    }
    public override void Update()
    {

        if (Random.Range(0, 100) < 10)
        {
            nextState = new MovingToPickup(npc, agent, anim, player, pickupLocation,dropoffLocation);
            stage = EVENT.EXIT;
        }
        //base.Update();
        // Transition logic can be added here
    }
    public override void Exit()
    {
        //reset idle trigger
        //anim.ResetTrigger("isIdle");
        base.Exit();
    }
}
#endregion


#region MovingToPickup
public class MovingToPickup : CraneState
{
    private float arriveAngle = 2f;
    private Transform craneSurface;
    private Transform craneArm;
    public MovingToPickup(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _pickupLocation, Vector3 _dropoffLocation)
        : base(_npc, _agent, _anim)
    {
        pickupLocation = _pickupLocation;
        dropoffLocation = _dropoffLocation;
        Debug.Log("Pickup Location set to: " + pickupLocation);
        Debug.Log("Dropoff Location set to: " + dropoffLocation);

        craneArm = npc.transform.Find("CraneArm");
        craneSurface = npc.transform.Find("CraneArm/handle/InteractSurface");
        Debug.Log("Crane arm found: " + craneArm.name);
        name = STATE.MOVINGTOPICKUP;

    }
    public override void Enter()
    {
        base.Enter();
        //agent.SetDestination(pickupLocation);
        //anim.SetTrigger("isMoving");
        Debug.Log("Crane is moving to Pickup Location.");
    }
    public override void Update()
    {

        if (craneSurface == null || craneArm == null)
        {
            Debug.LogError("Crane Arm is null!" + "Crane surface is null");
            return;
        }

        Vector3 dir = pickupLocation - craneSurface.position;
        dir.y = 0; // Keep only horizontal rotation

        if (dir.sqrMagnitude < 0.0001f)
            return; // No need to rotate if direction is too small

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        craneArm.rotation = Quaternion.Slerp(craneArm.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        float angle = Quaternion.Angle(craneArm.rotation, targetRotation);
        //Debug.Log("Angle to target: " + angle);

        if (angle <= arriveAngle)
        {
            nextState = new PickingUp(npc, agent, anim, player, pickupLocation, dropoffLocation);
            stage = EVENT.EXIT;
            return;
        }

        //base.Update();
    }
    public override void Exit()
    {
        //anim.ResetTrigger("isMoving");
        base.Exit();
    }
}
#endregion

#region PickingUp
public class PickingUp : CraneState
{
    private float pickUpTime = 3f;
    private float timer = 0f;

    public PickingUp(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _pickupLocation, Vector3 _dropoffLocation)
        : base(_npc, _agent, _anim)
    {
        pickupLocation = _pickupLocation;
        dropoffLocation = _dropoffLocation;
        Debug.Log("Pickup Location set to: " + pickupLocation);
        Debug.Log("Dropoff Location set to: " + dropoffLocation);

        name = STATE.PICKINGUP;
    }
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Crane is Picking Up Load.");
        // Set picking up trigger
        anim.SetTrigger("isPickingUp");
    }
    public override void Update()
    {
        // picking up process
        // After picking up, transition to MovingToDropoff state
        if (armHasLoad)
        {
            Debug.Log("Crane arm already has load.");
            return;
        }

        if(timer < pickUpTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            //if(CanSeePlayer() && !hasInitiatedAttack)
            //{
            //    nextState = new EmergencyStop(npc, agent, anim, player, pickupLocation, dropoffLocation);
            //    stage = EVENT.EXIT;
            //    return;
            //}

            nextState = new MovingToDropoff(npc, agent, anim, player, pickupLocation, dropoffLocation);
            stage = EVENT.EXIT;
            
        }

        //base.Update();
    }

    public override void Exit()
    {

        armHasLoad = true;
        // Reset picking up trigger
        anim.ResetTrigger("isPickingUp");
        timer = 0f;

        base.Exit();
    }
}
#endregion

#region MovingToDropoff
public class MovingToDropoff : CraneState
{
    private Transform craneSurface;
    private Transform craneArm;
    private float arriveAngle = 2f;
    private float rotateDegPerSec = 60f;

    public MovingToDropoff(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _pickupLocation, Vector3 _dropoffLocation)
        : base(_npc, _agent, _anim)
    {
        pickupLocation = _pickupLocation;
        dropoffLocation = _dropoffLocation;

        Debug.Log("Dropoff Location set to: " + dropoffLocation);
        craneSurface = npc.transform.Find("CraneArm/handle/InteractSurface");
        craneArm = npc.transform.Find("CraneArm");
        name = STATE.MOVINGTODROPPINGOFF;

    }
    public override void Enter()
    {
        //set moving to dropoff trigger
        //anim.SetTrigger("isMoving");
        base.Enter();
        Debug.Log("Crane is moving to Dropoff Location.");
    }
    public override void Update()
    {

        if (craneSurface == null || craneArm == null)
        {
            Debug.LogError("Crane Arm is null!" + "Crane surface is null");
            return;
        }

        //if (CanSeePlayer() && !hasInitiatedAttack)
        //{
        //    nextState = new EmergencyStop(npc, agent, anim, player, pickupLocation, dropoffLocation);
        //    stage = EVENT.EXIT;
        //    return;
        //}

        Vector3 dir = dropoffLocation - craneSurface.position;
        dir.y = 0; // Keep only horizontal rotation

        if (dir.sqrMagnitude < 0.0001f)
            return; // No need to rotate if direction is too small

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        craneArm.rotation = Quaternion.RotateTowards(craneArm.rotation, targetRotation, rotateDegPerSec * Time.deltaTime);

        float angle = Quaternion.Angle(craneArm.rotation, targetRotation);
        //Debug.Log("Angle to target: " + angle);

        if (angle <= arriveAngle)
        {
            nextState = new DroppingOff(npc, agent, anim, player, pickupLocation, dropoffLocation);
            stage = EVENT.EXIT;
            return;
        }
    }
    public override void Exit()
    {
        //anim.ResetTrigger("isMoving");
        base.Exit();
    }
}
#endregion

#region DroppingOff

public class DroppingOff : CraneState
{
    private float dropOffTime = 2f;
    private float timer = 0f;

    public DroppingOff(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _pickupLocation,Vector3 _dropoffLocation)
        : base(_npc, _agent, _anim)
    {
        pickupLocation = _pickupLocation;
        dropoffLocation = _dropoffLocation;

        name = STATE.DROPPINGOFF;

    }
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Crane is Dropping Off Load.");
        // Set dropping off trigger
        anim.SetTrigger("isDroppingOff");
    }
    public override void Update()
    {
        //if (CanSeePlayer() && !hasInitiatedAttack)
        //{
        //    nextState = new EmergencyStop(npc, agent, anim, player, pickupLocation, dropoffLocation);
        //    stage = EVENT.EXIT;
        //    return;
        //}

        // dropping off process
        if (timer < dropOffTime)
        { 
            timer += Time.deltaTime;
        }
        else
        {
            // After dropping off, transition to Idle state
            nextState = new Idle(npc, agent, anim, player, pickupLocation, dropoffLocation);
            stage = EVENT.EXIT;
        }

    }
    public override void Exit()
    {
        armHasLoad = false;
        // Reset dropping off trigger
        anim.ResetTrigger("isDroppingOff");
        base.Exit();
    }
}

#endregion

#region EmergencyStop
public class EmergencyStop : CraneState
{
    public EmergencyStop(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _pickupLocation, Vector3 _dropoffLocation)
        : base(_npc, _agent, _anim)
    {
        pickupLocation = _pickupLocation;
        dropoffLocation = _dropoffLocation;
        name = STATE.EMERGENCYSTOP;

    }
    public override void Enter()
    {
        base.Enter();
        // Set emergency stop trigger
        //anim.SetTrigger("isEmergencyStopping");
        Debug.Log("Crane is in Emergency Stop.");
    }
    public override void Update()
    {
        if(!CanAttackPlayer())
        {
            Debug.Log("Crane cannot attack player, switch to MovingToPickingUp state.");
            rotateSpeed = 50f;
            hasInitiatedAttack = true;
            nextState = new MovingToPickup(npc, agent, anim, player, pickupLocation, dropoffLocation);
            stage = EVENT.EXIT;
        }
        else
        {
            Debug.Log("Crane can attack player, switch to Pursue state.");
            nextState = new Pursue(npc, agent, anim, player, pickupLocation, dropoffLocation);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        // Reset emergency stop trigger
        //anim.ResetTrigger("isEmergencyStopping");
        base.Exit();
    }
}
#endregion

#region Pursue
public class Pursue : CraneState
{

    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _pickupLocation, Vector3 _dropoffLocation)
        : base(_npc, _agent, _anim)
    {
        pickupLocation = _pickupLocation;
        dropoffLocation = _dropoffLocation;
        name = STATE.PURSUE;
        rotateSpeed = 20f;

    }
    public override void Enter()
    {
        base.Enter();
        // Set pursue trigger
        //anim.SetTrigger("isPursuing");
        Debug.Log("Crane is Pursuing Player.");
    }
    public override void Update()
    {
        //float distance = Vector3.Distance(npc.transform.position, player.position);
        //if (distance < 0.1f)
        //{
        //    if (CanAttackPlayer())
        //    {
        //        // Transition to Attack state
        //        nextState = new Attack(npc, agent, anim, player,pickupLocation,dropoffLocation);
        //        stage = EVENT.EXIT;
        //    }
        //    else if (!CanAttackPlayer())
        //    {
        //        // Transition to Idle state
        //        nextState = new Idle(npc, agent, anim, player,pickupLocation,dropoffLocation);
        //        stage = EVENT.EXIT;
        //    }

        //}
        //else
        //{

        //}
    }
    public override void Exit()
    {
        // Reset pursue trigger
        //anim.ResetTrigger("isPursuing");
        base.Exit();
    }
}
#endregion

#region Attack
public class Attack : CraneState
{
    float rotationSpeed = 2f;
    AudioSource attackSound;
    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _pickupLocation, Vector3 _dropOffLocation)
        : base(_npc, _agent, _anim)
    {
        name = STATE.ATTACK;
        // Play attack sound
        //attackSound = npc.GetComponent<AudioSource>();
        Debug.Log("Crane is Attacking Player.");
    }
    public override void Enter()
    {
        // Set attack trigger
        //anim.SetTrigger("isAttacking");
        //sound attackSound.Play();
        //attackSound.Play();
        base.Enter();
    }
    public override void Update()
    {
        //// Rotate towards player
        //Vector3 direction = player.position - npc.transform.position;
        //float angle = Vector3.Angle(direction, npc.transform.forward);
        //// Ignore y-axis for rotation
        //direction.y = 0;

        //npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, 
        //                            Quaternion.LookRotation(direction), 
        //                            rotationSpeed * Time.deltaTime);
        //if(!CanSeePlayer())
        //{
        //    // Transition to Idle state
        //    nextState = new Idle(npc, agent, anim, player, pickupLocation, dropoffLocation);
        //    stage = EVENT.EXIT;
        //}

    }
    public override void Exit()
    {
        // Reset attack trigger & audio 
        //anim.ResetTrigger("isAttacking");     
        //attackSound.Stop();
        base.Exit();
    }
}

#endregion
