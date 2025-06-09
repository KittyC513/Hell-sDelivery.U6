using UnityEngine;
using UnityEngine.Events;


public class BreakableBox : MonoBehaviour
{
    //box or any object that can break on impact to the ground or being attacked by the player

    [SerializeField] public UnityEvent onBoxBreak;
    private Animator anim;

    public bool hasDestroyed = false;

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

        //freeze the object
        rb.constraints = RigidbodyConstraints.FreezePosition;

        //disable the collider
        colliderToDisable.enabled = false;

        //update animations to break the box
        //anim.SetTrigger("Destroyed");

        //destroy the box
        Destroy(this.gameObject, 0.05f);
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
            //the player has attacked the box
            if (!hasDestroyed) onBoxBreak?.Invoke();
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
            //the player has attacked the box
            if (!hasDestroyed) onBoxBreak?.Invoke();
        }
    }
}
