using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;

public class HellHoundStateMachine : StateManager <HellHoundStateMachine.HoundStates>
{
    public enum HoundStates { wander, chase, attack, cooldown, die }

    
    [Space, Header("General Variables")]
    [SerializeField] private float rotationSpeed = 15; //how fast the enemy rotates towards the target direction

    [Space, Header("Wander Variables")]
    [SerializeField] private float maxWanderDistance; //how far away the wander math can pick a spot
    [SerializeField] private float wanderTime = 4; //how long a wander path lasts until a new one is chosen

    [SerializeField] private float playerDetectionRadius = 5; //how far away in a sphere can the player be deteceted
    [SerializeField] private float visionConeAngle = 45; //a vision cone in front of the enemy that detects the player

    [SerializeField] private LayerMask playerMask; //the layermask attached to the player

    [Space, Header("Attack Variables")]
    [SerializeField] private float attackDetectionRange = 1; //how far away should the player be until an attack starts
    [SerializeField] private float attackDelayTime = 0.5f; //how long before the attack hitbox comes out 
    [SerializeField] private float attackDuration = 0.2f; //how long the attack hitbox lasts
    [SerializeField] private float attackCooldownTime = 1f; //how long the enemy stalls after attacking
    [SerializeField] private GameObject attackHitboxObj;
    [HideInInspector] public bool shouldRotate = true;

    [Space, Header("Ground Check Variables")]
    [SerializeField] private float groundCheckDist = 0.15f; //how far down the ground can be detected
    [SerializeField] private LayerMask groundMask; //the ground layer mask
    [SerializeField] private bool showRadius = true; //use this to see player detection radius and ground ray with gizmos
    private bool grounded = false;

    [Space, Header("References")]
    [SerializeField] private Rigidbody rb; //the rigidbody attached to the enemy
    [SerializeField] private EnemyHealth eHealth; //the enemy health script
    [SerializeField] private NavMeshAgent navAgent; //the nav agent

    private GameObject targetPlayer; //the current target player

    private bool addKnockback = false; //is knockback being added

    //accessible variables
    public NavMeshAgent NavAgent { get { return navAgent; } }
    public float MaxWanderDistance { get { return maxWanderDistance; } }
    public float WanderTime {  get { return wanderTime; } }
    public GameObject TargetPlayer {  get { return targetPlayer; } }
    public Rigidbody RB { get { return rb; } }
    public bool AddKnockback {  get { return addKnockback; } }
    public float VisionConeAngle {  get { return visionConeAngle; } }
    public float PlayerDetectionRadius { get { return playerDetectionRadius; } }
    public float CooldownTime { get { return attackCooldownTime; } }
    public float AttackDetectionRange {  get { return attackDetectionRange; } }
    public float AttackDelayTime {  get { return attackDelayTime; } }
    public float AttackDuration { get { return attackDuration; } }

    private void Awake()
    {
        //initialize states for the state machine
        states.Add(HoundStates.wander, new HellHoundWander(HoundStates.wander, this));
        states.Add(HoundStates.chase, new HellHoundChase(HoundStates.chase, this));
        states.Add(HoundStates.attack, new HellHoundAttack(HoundStates.attack, this));
        states.Add(HoundStates.cooldown, new HellHoundCooldown(HoundStates.cooldown, this));

        //set current state
        currentState = states[HoundStates.wander];
        TransitionToState(HoundStates.wander);

        //we handle rotation seperately from the navmesh
        navAgent.updateRotation = false;

        //get the enemy health component (this is required)
        if (eHealth == null) eHealth = GetComponent<EnemyHealth>();
        
    }

    private void OnEnable()
    {
        //add the knockback function to the take damage event
        eHealth.onTakeDamage += StartKnockback;
    }

    private void OnDisable()
    {
        eHealth.onTakeDamage -= StartKnockback;
    }

    private void LateUpdate()
    {
        //if we are not grounded apply gravity until ground is reached, this stops the enemy teleporting to the ground
        if(!DetectGround())
        {
            //nav agent must be disabled to use the rigidbody's gravity
            navAgent.updatePosition = false;
            rb.useGravity = true;
        }
      
    }

    private void FixedUpdate()
    {
        //this is how the enemy looks towards its destination
        if (shouldRotate) RotateTowards((navAgent.destination - navAgent.transform.position).normalized, rotationSpeed);
    }


    private void OnDrawGizmosSelected()
    {
        if (showRadius)
        {
            Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDist, transform.position.z));
        }
    }

    public GameObject DetectPlayer(float range, float angle)
    {
        //this function detects the player within a sphere range and an angle vision cone in front of the enemy
        Collider[] players = Physics.OverlapSphere(transform.position, range, playerMask);

        if (players.Length > 0)
        {
            foreach (Collider collider in players)
            {
                Collider player = collider;

                //the direction towards the player from the enemy
                Vector3 targetDir = player.transform.position - transform.position;

                //if the angle between the forward direction of the enemy and the player is less than the vision cone angle
                if (Vector3.Angle(transform.forward, targetDir) < angle)
                {
                    //set the target player to the detected player
                    targetPlayer = player.gameObject;
                    return targetPlayer;
                }
            }
        }

        return null;
    }

    private bool DetectGround()
    {
        //where the raycast towards the ground starts
        Vector3 rayStartPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //check downwards for ground, this is used for applying gravity when not on the navmesh
        if (Physics.Raycast(rayStartPos, Vector3.down, groundCheckDist, groundMask))
        {
            if (!addKnockback && !navAgent.updatePosition && !grounded)
            {
                navAgent.updatePosition = true;
                rb.useGravity = false;
            }
            grounded = true;


            return true;
        }

        grounded = false;
        return false;
    }

    public void ClearPlayer()
    {
        //reset the target player
        targetPlayer = null;
    }
  

    private void OverrideState(HoundStates newState)
    {
        TransitionToState(newState);
    }

    public void StartKnockback(Vector3 force)
    {
        //a way to call the coroutine without needing monobehaviour or the system namespace
        StartCoroutine(ApplyKnockback(force));
    }

    public IEnumerator ApplyKnockback(Vector3 force)
    {
        //knockback is being added until this bool is false
        addKnockback = true;

        //wait for 1 frame just in case anything returns with errors
        yield return null;

        //disable the navmesh to enable the rigidbody forces
        navAgent.updatePosition = false;
        rb.useGravity = true;

        //add force
        rb.AddForce(force, ForceMode.Impulse);

        //wait until the force is actually added in fixed update
        yield return new WaitForFixedUpdate();

        //don't go past this line until the velocity of the rigidbody reaches a low number
        yield return new WaitUntil(() => rb.linearVelocity.magnitude < 0.05f && navAgent.isOnNavMesh);

        //stun time before moving again
        yield return new WaitForSeconds(0.1f);

        //reset the velocity values
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //enable navmesh and disable rb physics
        rb.useGravity = false;

        //make sure the navmesh knows where the enemy is
        navAgent.Warp(transform.position);
        navAgent.updatePosition = true;

        //knockback is no longer being added
        addKnockback = false;
        yield return null;
    }

    public void ResetKnockback()
    {
        //this function can be used to reset all the knockback variables if any override is needed
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;

        navAgent.Warp(transform.position);
        navAgent.enabled = true;
    }

    private void RotateTowards(Vector3 direction, float rotationSpeed)
    { 
         //get our desired direction ignoring y 
        direction = new Vector3(direction.x, 0, direction.z);

        if (direction.magnitude > 0)
        {
            //calculate our desired rotation
            Quaternion toRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            //use rotate towards to rotate to our desired position by our rotation speed rather than all at once
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, (rotationSpeed) * Time.fixedDeltaTime);
        }

    }

    public void ToggleAttackHitbox(bool state)
    {
        attackHitboxObj.SetActive(state);
    }

}

