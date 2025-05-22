using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    protected GameObject targetPlayer;
    protected bool addKnockback = false;

    [Space, Header("Enemy Base Variables")]
    [SerializeField] protected float groundCheckDist = 0.8f;
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected float deathTime = 0.5f;
    [SerializeField] protected float rotationSpeed = 15; //how fast the enemy rotates towards the target direction
    protected bool grounded;
    [SerializeField] protected float playerDetectionRadius = 5; //how far away in a sphere can the player be detected

    [Space, Header("Enemy Base References")]
    [SerializeField] protected LayerMask playerMask;
    [SerializeField] protected Rigidbody rb; //the rigidbody attached to the enemy
    [SerializeField] protected NavMeshAgent navAgent; //the nav agent
    [SerializeField] protected EnemyHealth eHealth; //the enemy health script

    private bool droppedMoney = false;
    protected bool isDead = false;

    public NavMeshAgent NavAgent { get {  return navAgent; } }
    public GameObject TargetPlayer { get { return targetPlayer; } }
    public virtual GameObject DetectPlayer(float range, float angle)
    {
        //if a player is detected in the sphere
        Collider[] players = Physics.OverlapSphere(transform.position, range, playerMask);
        if (players.Length > 0)
        {
            Collider player = players[0];

            //the direction towards the player from the enemy
            Vector3 targetDir = player.transform.position - transform.position;

            //if the angle between the forward direction of the enemy and the player is less than the vision cone angle
            if (Vector3.Angle(transform.forward, targetDir) < angle)
            {
                //target player and start chasing

                targetPlayer = players[0].gameObject;
                return targetPlayer;
            }
        }

        return null;
    }

    protected bool DetectGround()
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
        targetPlayer = null;
    }

    public void StartKnockback(Vector3 force)
    {
        StartCoroutine(ApplyKnockback(force, navAgent));
    }

    public IEnumerator ApplyKnockback(Vector3 force, NavMeshAgent navAgent)
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
        yield return new WaitUntil(() => rb.linearVelocity.magnitude < 0.05f && navAgent.isOnNavMesh && grounded);

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

    public void RotateTowards(Vector3 direction, float rotationSpeed)
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

    protected void OnEnemyDeath()
    {
        //disable navmesh
        navAgent.updatePosition = false;
        //keep rigidbody active but stop adding forces / cancel knockback 
        isDead = true;
        //drop coins or something at random positions
        //DropMoney();
        //update animations


        //poof / destroy object after timer / animation finishes
        Destroy(this.gameObject, deathTime);
    }

    protected void DropMoney()
    {
        //get random value of money between range

        //drop each piece at a random direction + upwards slightly
    }
}
