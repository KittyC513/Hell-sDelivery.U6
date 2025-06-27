using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [Space, Header("Bounce Values")]
    [SerializeField] private float bounceForce = 15;
    [SerializeField] private bool useSpring = false;
    [SerializeField] private Vector3 bounceDir = Vector3.up;
    [SerializeField] private bool useRelativeDir = false;
    private Vector3 startingScale;
    private float bounceValue;

    [Space, Header("Spring Anim Values")]
    [SerializeField] private GameObject springObject;
    [SerializeField] private float frequency = 15;
    [SerializeField] private float damping = 0.5f;
    [SerializeField] private float bounceScale = 0.8f;
    [SerializeField] private float minimumScale = 0.35f;
    private float scale;
    private float velocity;
    private float targetScale = 1;

    SpringUtils.tDampedSpringMotionParams temp;
    private void Start()
    {
        temp = new SpringUtils.tDampedSpringMotionParams();
        startingScale = springObject.transform.localScale;
    }

    //Bounces a rigidbody passed into this function
    public void BounceObject(Rigidbody rb, float downwardForce)
    {
        float downForce = downwardForce;

        //A toggle used to enable the bounce direction being relative to where the player bounced from 
        if (useRelativeDir)
        {
            //get the direction from the bounce object to the player to determine direction
            bounceDir = rb.transform.position - transform.position;
            bounceDir.Normalize();
        }

        //get a y force value based on how fast the player is moving in the y direction
        float yForce = Mathf.Abs((downwardForce) / 25);

        //clamp that force for a maximum of 3x the bounce value
        Mathf.Clamp(yForce, 0.1f, 3);
        
        //set a bounce velocity direction
        Vector3 bounceVel = new Vector3(bounceDir.x, bounceDir.y * yForce, bounceDir.z);

        rb.AddForce(bounceVel * (bounceForce), ForceMode.Impulse);

        //used to scale how much the spring animation is applied
        float animScale = Mathf.Abs(downForce) / 115;
        
        if (bounceScale - animScale < minimumScale)
        {
            scale = minimumScale;
        }
        else
        {
            scale = bounceScale - animScale;
        }
    }


    private void Update()
    {
        if (useSpring)
        {
            //spring utility, makes this object bounce when jumped on 
            SpringUtils.CalcDampedSpringMotionParams(ref temp, Time.deltaTime, frequency, damping);
            SpringUtils.UpdateDampedSpringMotion(ref scale, ref velocity, targetScale, temp);
            springObject.transform.localScale = startingScale * scale;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameObject player = collision.collider.gameObject;
            PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();

            if (stateMachine.showCurrentState != PlayerStateMachine.PlayerStates.jump)
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                BounceObject(rb, rb.linearVelocity.y);
            }
           
        }
        else
        {
            Rigidbody rb = collision.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                BounceObject(rb, 20);
            }
        }
    }

}
