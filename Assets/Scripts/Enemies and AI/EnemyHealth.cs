using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //an event for when the enemy takes damage, when adding a function make sure it has a value for the dmg value
    public delegate void OnTakeDamage();
    public OnTakeDamage onTakeDamage;

    //an event for when the enemy dies
    public delegate void OnEnemyDeath();
    public OnEnemyDeath onEnemyDeath;

    [SerializeField] private int maxHealth = 2; //maximum health of the enemy
    [SerializeField] private int currentHealth = 0; //current health of the enemy

    [SerializeField] private float invulTime = 0.2f; //how long is the enemy not able to take damage after taking a hit
    private float invulTemp = 0;

    private bool invulnerable = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        
    }

    //called when this script takes damage, not subbed to the event because this function needs a dmg value whereas any other reaction to taking damage wouldn't
    private void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            onEnemyDeath?.Invoke();
        }

        invulTemp = 0;
    }

    private void Update()
    {
        if (invulTemp < invulTime)
        {
            invulTemp += Time.deltaTime;
            invulnerable = true;
        }
        else
        {
            //no longer invul
            invulnerable = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack") && !invulnerable)
        {
            onTakeDamage?.Invoke();
            TakeDamage(1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerAttack") && !invulnerable)
        {
            onTakeDamage?.Invoke();
            TakeDamage(1);
        }
    }


}
