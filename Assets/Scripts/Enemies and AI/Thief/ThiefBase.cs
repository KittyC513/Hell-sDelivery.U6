using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ThiefBase : EnemyBase
{
    public bool shouldRotate = true;
    [SerializeField] private bool showRadius;
    [HideInInspector] public bool hasPackage = false;
    [SerializeField] private ThiefStateMachine thiefMachine;

    [Space, Header("Sneaking Variables")]
    [SerializeField] private float sneakSpeed = 10;
    [SerializeField] private float stealRange = 1;
    private float defaultSpeed;

    [Space, Header("Idle Variables")]
    [SerializeField] private float wanderTime = 5;
    [SerializeField] private float maxWanderDistance = 3;

    [Space, Header("Steal Variables")]
    [SerializeField] private float stealDelay = 0.5f;
    [SerializeField] private float stealTime = 0.2f;
    

    public Rigidbody RB { get { return rb; } }
    public float StealRange { get { return stealRange; } }
    public float SneakSpeed { get { return sneakSpeed; } }
    public float WanderTime { get { return wanderTime; } }
    public float MaxWanderDistance { get { return maxWanderDistance; } }
    public float DefaultSpeed { get { return defaultSpeed; } }
    public float StealDelay { get  { return stealDelay; } }
    public float StealTime { get { return stealTime; } }

    
    private void Start()
    {
        //we handle rotation seperately from the navmesh
        navAgent.updateRotation = false;
        defaultSpeed = navAgent.speed;
    }

    private void Update()
    {
        if (isDead)
        {
            navAgent.updatePosition = false;
            shouldRotate = false;
        }
    }
    private void FixedUpdate()
    {
        //this is how the enemy looks towards its destination
        if (shouldRotate) RotateTowards((navAgent.destination - navAgent.transform.position).normalized, rotationSpeed);

        //if we are not grounded apply gravity until ground is reached, this stops the enemy teleporting to the ground
        if (!DetectGround())
        {
            //nav agent must be disabled to use the rigidbody's gravity
            navAgent.updatePosition = false;
            rb.useGravity = true;
        }
    }


    private void OnEnable()
    {
        //add the knockback function to the take damage event
        eHealth.onTakeDamage += StartKnockback;
        eHealth.onEnemyDeath += OnEnemyDeath;
        eHealth.onTakeDamage += SetRunState;
    }


    private void OnDisable()
    {
        eHealth.onTakeDamage -= StartKnockback;
        eHealth.onEnemyDeath -= OnEnemyDeath;
        eHealth.onTakeDamage -= SetRunState;
    }

    private void OnDrawGizmosSelected()
    {
        if (showRadius)
        {
            Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDist, transform.position.z));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stealRange);
           
        }
    }

    private void SetRunState(Vector3 dir)
    {
        thiefMachine.OverrideState(ThiefStateMachine.ThiefStates.run);
    }
}
