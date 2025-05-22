using UnityEngine;

public class ThiefBase : EnemyBase
{
    private bool shouldRotate;
    [SerializeField] private bool showRadius;
    [HideInInspector] public bool hasPackage = false;

    [SerializeField] private float sneakSpeed = 10;
    [SerializeField] private float stealRange = 1;

    [SerializeField] private float stealDelayTime = 0.4f;
    [SerializeField] private float stealCheckTime = 0.15f;

    public float StealRange { get { return stealRange; } }
    public float SneakSpeed { get { return sneakSpeed; } }


    private void Start()
    {
        //we handle rotation seperately from the navmesh
        navAgent.updateRotation = false;
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
}
