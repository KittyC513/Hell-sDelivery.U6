using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;

public class HellHoundStateMachine : StateManager <HellHoundStateMachine.HoundStates>
{
    public enum HoundStates { wander, chase, attack, hurt, die }

    public EnemyHealth enemyHealth;
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private float rotationSpeed = 500;

    [Space, Header("Wander Variables")]
    [SerializeField] private float maxWanderDistance;
    [SerializeField] private float wanderTime = 4;

    [SerializeField] private float playerDetectionRadius = 5;
    [SerializeField] private float visionConeAngle = 45;

    [SerializeField] private bool showRadius = true;
    [SerializeField] private LayerMask playerMask;

    private GameObject targetPlayer;
    [SerializeField] private Rigidbody rb;

    private bool addKnockback = false;

    //accessible variables
    public NavMeshAgent NavAgent { get { return navAgent; } }
    public float MaxWanderDistance { get { return maxWanderDistance; } }
    public float WanderTime {  get { return wanderTime; } }
    public GameObject TargetPlayer {  get { return targetPlayer; } }
    public Rigidbody RB { get { return rb; } }
    public bool AddKnockback {  get { return addKnockback; } }


    public float PlayerDetectionRadius { get { return playerDetectionRadius; } }

    private void Awake()
    {
        states.Add(HoundStates.wander, new HellHoundWander(HoundStates.wander, this));
        states.Add(HoundStates.chase, new HellHoundChase(HoundStates.chase, this));

        currentState = states[HoundStates.wander];
        TransitionToState(HoundStates.wander);
        navAgent.updateRotation = false;
    }

    private void FixedUpdate()
    {
        RotateTowards((navAgent.destination - navAgent.transform.position).normalized, rotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        if (showRadius) Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
        Gizmos.DrawRay(transform.position, transform.forward);
    }

    public GameObject DetectPlayer()
    {
        //if a player is detected in the sphere
        Collider[] players = Physics.OverlapSphere(transform.position, playerDetectionRadius, playerMask);
        if (players.Length > 0)
        {
            Collider player = players[0];

            Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            Vector3 houndXZ = new Vector3(transform.position.x, 0, transform.position.y);

            //if the angle between the forward direction of the enemy and the player is less than the vision cone angle
            if (Vector3.Angle(transform.forward, playerXZ) < visionConeAngle)
            {
                //target player and start chasing
                targetPlayer = players[0].gameObject;
                return targetPlayer;
            }
        }

        return null;
    }

    public void ClearPlayer()
    {
        targetPlayer = null;
    }
  

    private void OverrideState(HoundStates newState)
    {
        TransitionToState(newState);
    }

    public void StartKnockback(Vector3 force)
    {
        StartCoroutine(ApplyKnockback(force));
    }

    public IEnumerator ApplyKnockback(Vector3 force)
    {
        Vector3 startingPos = transform.position;
        addKnockback = true;
        //wait for 1 frame just in case anything returns with errors
        yield return null;

        navAgent.updatePosition = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.linearVelocity.magnitude < 0.05f && navAgent.isOnNavMesh);

        yield return new WaitForSeconds(0.1f);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        navAgent.Warp(transform.position);
        navAgent.updatePosition = true;
        addKnockback = false;
        yield return null;
    }

    public void ResetKnockback()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
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


}

