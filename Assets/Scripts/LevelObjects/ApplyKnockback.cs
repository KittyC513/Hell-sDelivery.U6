using UnityEngine;
using static Health;

public class ApplyKnockback : MonoBehaviour 
{
    //a simple script to apply knockbacks to an object based on collision velocity and multipliers

    [SerializeField] private bool applyCollisionKnockback = true;
    [SerializeField] public KnockbackForce[] knockbackForces;

    private Rigidbody rb;
    public delegate void OnKnockback(Vector3 force);
    public OnKnockback onKnockback;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (applyCollisionKnockback)
        {
            for (int i = 0; i < knockbackForces.Length; i++)
            {
                if (collision.collider.tag == knockbackForces[i].tag)
                {
                    Rigidbody rb2 = collision.collider.GetComponent<Rigidbody>();
                    float vel = rb2.linearVelocity.magnitude * rb2.mass;
                    Debug.Log("vel: " + vel*rb.mass);
                    vel = Mathf.Clamp(vel, 0, 8);

                    if (vel > knockbackForces[i].requiredVel)
                    {
                        Vector3 knockDir = (transform.position - collision.collider.transform.position).normalized;
                        knockDir = knockDir.normalized;
                        Vector3 force = knockDir * (vel * knockbackForces[i].knockbackMultiplier);
                        force = new Vector3(force.x, vel * knockbackForces[i].knockbackMultiplier, force.z);
                        rb.AddForce(force, ForceMode.Impulse);
                        onKnockback.Invoke(force);
                        //Debug.Log("Knockback: " + knockDir * (vel * knockbackForces[i].knockbackMultiplier));
                    }
                }
            }
        }
        
    }
}

[System.Serializable]
public class KnockbackForce
{
    public string tag = "default";
    public float requiredVel = 10;
    public float knockbackMultiplier = 5;
}
