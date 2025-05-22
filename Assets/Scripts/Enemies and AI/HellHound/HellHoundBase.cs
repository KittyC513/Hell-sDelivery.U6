using UnityEngine;
using UnityEngine.AI;

public class HellHoundBase : EnemyBase
{
    [Space, Header("Wander Variables")]
    [SerializeField] private float maxWanderDistance; //how far away the wander math can pick a spot
    [SerializeField] private float wanderTime = 4; //how long a wander path lasts until a new one is chosen
    [SerializeField] private float visionConeAngle = 45; //a vision cone in front of the enemy that detects the player

    [Space, Header("Attack Variables")]
    [SerializeField] private float attackDetectionRange = 1; //how far away should the player be until an attack starts
    [SerializeField] private float attackDelayTime = 0.5f; //how long before the attack hitbox comes out 
    [SerializeField] private float attackDuration = 0.2f; //how long the attack hitbox lasts
    [SerializeField] private float attackCooldownTime = 1f; //how long the enemy stalls after attacking
    [SerializeField] private GameObject attackHitboxObj;
    [HideInInspector] public bool shouldRotate = true;

    [SerializeField] private bool showRadius = true; //use this to see player detection radius and ground ray with gizmos

    //accessible variables
    public NavMeshAgent NavAgent { get { return navAgent; } }
    public float MaxWanderDistance { get { return maxWanderDistance; } }
    public float WanderTime { get { return wanderTime; } }
    public GameObject TargetPlayer { get { return targetPlayer; } }
    public Rigidbody RB { get { return rb; } }
    public bool AddKnockback { get { return addKnockback; } }
    public float VisionConeAngle { get { return visionConeAngle; } }
    public float PlayerDetectionRadius { get { return playerDetectionRadius; } }
    public float CooldownTime { get { return attackCooldownTime; } }
    public float AttackDetectionRange { get { return attackDetectionRange; } }
    public float AttackDelayTime { get { return attackDelayTime; } }
    public float AttackDuration { get { return attackDuration; } }

    private void Awake()
    {
        //get the enemy health component 
        if (eHealth == null) eHealth = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        //we handle rotation seperately from the navmesh
        navAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        //add the knockback function to the take damage event
        eHealth.onTakeDamage += StartKnockback;
        eHealth.onEnemyDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        eHealth.onTakeDamage -= StartKnockback;
        eHealth.onEnemyDeath -= OnEnemyDeath;
    }

    private void Update()
    {
        //if we are not grounded apply gravity until ground is reached, this stops the enemy teleporting to the ground
        if (!DetectGround())
        {
            Debug.Log("NOT GROUNDED");
            //nav agent must be disabled to use the rigidbody's gravity
            navAgent.updatePosition = false;
            rb.useGravity = true;
        }

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
    }

    public void ToggleAttackHitbox(bool state)
    {
        attackHitboxObj.SetActive(state);
    }


    private void OnDrawGizmosSelected()
    {
        if (showRadius)
        {
            Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDist, transform.position.z));
        }
    }


}
