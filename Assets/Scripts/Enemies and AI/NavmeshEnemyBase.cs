using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NavmeshEnemyBase : MonoBehaviour
{

    protected GameObject targetPlayer;
    protected bool addKnockback = false;

    [Space, Header("GroundCheckVariables")]
    [SerializeField] private float groundCheckDist = 0.15f;
    [SerializeField] private LayerMask groundMask;

    public virtual GameObject DetectPlayer(float range, float angle, LayerMask playerMask)
    {
        //if a player is detected in the sphere
        Collider[] players = Physics.OverlapSphere(transform.position, range, playerMask);
        if (players.Length > 0)
        {
            Collider player = players[0];

            Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            Vector3 targetDir = playerXZ - transform.position;

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

    public bool DetectGround(NavMeshAgent agent, LayerMask groundMask, float raycastDist)
    {
        Vector3 rayStartPos = new Vector3(transform.position.x, transform.position.y - agent.height / 2, transform.position.z);
        if (Physics.Raycast(rayStartPos, Vector3.down, raycastDist, groundMask))
        {
            return true;
        }

        return false;
    }

    public void ClearPlayer()
    {
        targetPlayer = null;
    }

    public void StartKnockback(Vector3 force, Rigidbody rb, NavMeshAgent agent)
    {
        StartCoroutine(ApplyKnockback(force, rb, agent));
    }

    public IEnumerator ApplyKnockback(Vector3 force, Rigidbody rb, NavMeshAgent navAgent)
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


}
