using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;


public class BreakableBox : MonoBehaviour
{
    //box or any object that can break on impact to the ground or being attacked by the player

    [SerializeField] public UnityEvent onBoxBreak;
    private Animator anim;

    public bool hasDestroyed = false;
    [SerializeField] private bool destroyOnBreak = true;
    [SerializeField] private bool knockback = false;
    [SerializeField] private float knockbackForce = 10;

    private Rigidbody rb;

    private Collider colliderToDisable;

    [SerializeField] private float destroyableSpeed = 2;

    private void Awake()
    {
        colliderToDisable = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        onBoxBreak.AddListener(DestroyBox);
    }

    private void OnDisable()
    {
        onBoxBreak.RemoveListener(DestroyBox);
    }

    private void DestroyBox()
    {
        //this box has been destroyed
        hasDestroyed = true;
        //update animations to break the box
        //anim.SetTrigger("Destroyed");
       
        if (destroyOnBreak)
        {
            //freeze the object
            rb.constraints = RigidbodyConstraints.FreezePosition;
            //disable the collider
            colliderToDisable.enabled = false;
            //destroy the box
            Destroy(this.gameObject, 0.05f);
        }
    }

    private void CheckImpactDestroy()
    {
        //check if the speed of the box at impact is greater than a certain value and if it is destroy it
        if (rb.linearVelocity.magnitude > destroyableSpeed)
        {
            if (!hasDestroyed) onBoxBreak?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerAttack"))
        {
            ApplyKnockback(collision.collider.gameObject);
            //the player has attacked the box
            if (!hasDestroyed)
            {
                onBoxBreak?.Invoke();
            }
        }
        else //this is used to determine if the box has smashed into a surface hard enough to break
        {
            CheckImpactDestroy();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            ApplyKnockback(other.gameObject);
            //the player has attacked the box
            if (!hasDestroyed) onBoxBreak?.Invoke();
        }
    }

    private void ApplyKnockback(GameObject attacker)
    {
        if (knockback)
        {
            Vector3 dir = (transform.position - attacker.transform.position).normalized;
            dir = new Vector3(dir.x, dir.y+1f, dir.z);
            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);

        }
    }
}
