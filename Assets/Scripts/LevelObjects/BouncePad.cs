using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private float bounceForce = 8;
    public void BounceObject(Rigidbody rb)
    {
        //list of layers this object can bounce
        //check for a rigidbody
        //if there is a rigidbody get their downward force 
        float downForce = rb.linearVelocity.y;
        
        //apply bounce by downward force multiplier upwards
        if (downForce < 0)
        {
            float absForce = Mathf.Abs(downForce) / 25;
            
            rb.AddForce(Vector3.up * (absForce * bounceForce), ForceMode.Impulse);
        }
        else
        {
            float absForce = 0.1f;

            rb.AddForce(Vector3.up * (absForce * bounceForce), ForceMode.Impulse);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody _rb = other.GetComponent<Rigidbody>();
            if (_rb != null)
            {
                //bounce the player and reset the jumps
                BounceObject(_rb);

                //jumps need to be reset because sometimes ground isn't detected by the time this function runs
                //which means the player jumps don't reset when the player is moving too fast
                PlayerController pControl = other.GetComponent<PlayerController>();
                pControl.remainingJumps = pControl.MaxJumps;
            }
        }
    }
}
