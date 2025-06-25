//using System.Drawing.Text;
using UnityEngine;

public class Health : MonoBehaviour
{
    //an event for when the enemy takes damage, when adding a function make sure it has a value for the dmg value
    public delegate void OnTakeDamage(Vector3 dir);
    public OnTakeDamage onTakeDamage;

    //an event for when the enemy dies
    public delegate void OnDeath();
    public OnDeath onDeath;

    private bool dead = false;

    [SerializeField] private int maxHealth = 2; //maximum health of the enemy
    [SerializeField] private int currentHealth = 0; //current health of the enemy

    [SerializeField] private float invulTime = 0.2f; //how long is the enemy not able to take damage after taking a hit
    private float invulTemp = 0;

    [SerializeField] private DropMoney dropMoney; //the script that allows this object to drop money on death

    public bool invulnerable = false;

    [SerializeField] public DamageDealer[] damageDealers;

    private void Awake()
    {
        currentHealth = maxHealth;
        //tagHandle = TagHandle.GetExistingTag(damageString);
    }

    private void OnEnable()
    {
        if (dropMoney != null) onDeath += dropMoney.SpawnCurrency;
    }

    private void OnDisable()
    {
        if (dropMoney != null) onDeath -= dropMoney.SpawnCurrency;
    }

    //called when this script takes damage, not subbed to the event because this function needs a dmg value whereas any other reaction to taking damage wouldn't
    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0 && !dead)
        {
            onDeath?.Invoke();
            dead = true;
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
        if (!invulnerable)
        {
            for (int i = 0; i < damageDealers.Length; i++)
            {
                if (other.tag == damageDealers[i].tag)
                {
                    Vector3 knockDir = (transform.position - other.transform.position).normalized;
                    knockDir = knockDir.normalized;
                    onTakeDamage?.Invoke(knockDir * damageDealers[i].knockbackForce);
                    TakeDamage(damageDealers[i].damage);
                }
            }
           
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!invulnerable)
        {
            for (int i = 0; i < damageDealers.Length; i++)
            {
                if (collision.collider.tag == damageDealers[i].tag)
                {
                    Vector3 knockDir = (transform.position - collision.collider.transform.position).normalized;
                    knockDir = knockDir.normalized;
                    onTakeDamage?.Invoke(knockDir * damageDealers[i].knockbackForce);
                    TakeDamage(damageDealers[i].damage);
                }
            }

        }
    }


}

[System.Serializable]
public struct DamageDealer
{
    [SerializeField] public string tag;
    [SerializeField] public int damage;
    [SerializeField] public float knockbackForce;
}
