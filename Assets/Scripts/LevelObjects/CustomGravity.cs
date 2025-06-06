using UnityEngine;

//a very simple script to quickly apply downwards force with a custom gravity value instead of using unity's built in gravity
public class CustomGravity : MonoBehaviour 
{
    [SerializeField] private float gravity = 9.81f;

    public enum PhysicsStyle { Rigidbody, Transform };
    [SerializeField] public PhysicsStyle physicsStyle = PhysicsStyle.Rigidbody;

    private Rigidbody rb;

    private void Awake()
    {

        switch (physicsStyle)
        {
            case PhysicsStyle.Rigidbody:
                rb = GetComponent<Rigidbody>();
                break;
        }

    }
    private void FixedUpdate()
    {
        switch (physicsStyle)
        {
            case PhysicsStyle.Rigidbody:
                rb.AddForce(Vector3.down * gravity, ForceMode.Force);
                break;
            case PhysicsStyle.Transform:
                this.transform.position += new Vector3(0, -gravity, 0) * Time.deltaTime;
                break;
        }
    }
}
