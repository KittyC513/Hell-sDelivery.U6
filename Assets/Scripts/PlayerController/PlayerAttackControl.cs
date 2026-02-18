using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAttackControl : MonoBehaviour
{
    [Space, Header("Attack Variables")]
    [SerializeField] private float totalAttackTime = 0.2f; //how long the attack physically lasts
    [SerializeField] private float attackHitboxTime = 0.4f; //how long the hitbox of the attack lasts
    private float hitboxTimer = 0; //the timer for the hitbox toggle

    [SerializeField] private float attackCooldown = 0.5f; //how long is the cooldown between attacks
    private float attackCooldownTimer = 0; //a timer for the attack cooldown, gets reset at the END of an attack
    
    [SerializeField] public bool canAttack = true; //is the attack off cooldown
    [SerializeField] private PlayerController pControl;

    [SerializeField] private GameObject attackHitbox; //the game object the attack hitbox is attached to
    [SerializeField] private float attackSpeedBoost = 130f; //how much speed is added to the player when attacking while grounded

    [Space, Header("Air Stall Variables")]
    [SerializeField] private float maximumVerticalSpeed = 5;
    [SerializeField] private float stallVelocity = 50;

    [HideInInspector] public bool shouldRefresh = false;

    [SerializeField] public ParticleSystem attackParticles;
    private bool _grounded;

    //Attack Variables
    public float AttackTime { get { return totalAttackTime; } }
    public float AttackSpeedBoost { get { return attackSpeedBoost; } }

    //Stall Variables
    public float MaximumVerticalSpeed {  get { return maximumVerticalSpeed; } }
    public float StallVelocity { get { return stallVelocity; } }

    private void Start()
    {
        pControl.leftTheGround += RefreshAttack;
    }
    private void Update()
    {
        AttackCooldown();
        UpdateAttackHitbox();
    }

    private void AttackCooldown()
    {
        //if im not grounded
        //and i cannot attack any longer
        //once i become grounded refresh my attack 
        if (!pControl.Grounded && !canAttack)
        {
            shouldRefresh = true;
        }

        if (shouldRefresh && pControl.Grounded)
        {
            canAttack = true;
            attackCooldownTimer = attackCooldown;
            shouldRefresh = false;
        }



        //checks if the cooldown between attacks is over and the player can attack again
        if (attackCooldownTimer < attackCooldown)
        {
            attackCooldownTimer += Time.deltaTime;
        }

        if (attackCooldownTimer >= attackCooldown)
        {
            if (pControl.Grounded)
            {
                canAttack = true;
                _grounded = true;
            }
        }
        else
        {
            canAttack = false;
        }

    }

    public void RefreshAttack()
    {
        canAttack = true;
        attackCooldownTimer = attackCooldown;
        shouldRefresh = false;
    }

    public void ResetAttackCooldown()
    {
        attackCooldownTimer = 0;
    }

    //this is called when the attack state starts
    public void ResetHitboxTime()
    {
        hitboxTimer = 0;
    }

    public void UpdateAttackHitbox()
    {
        //activate and deactivate the hitbox based on the hitbox timer
        if (hitboxTimer < attackHitboxTime)
        {
            hitboxTimer += Time.deltaTime;
        }

        if (hitboxTimer >= attackHitboxTime)
        {
            attackHitbox.SetActive(false);
        }
        else
        {
            attackHitbox.SetActive(true);
        }
    }

    
}
