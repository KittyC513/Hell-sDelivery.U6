using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class LooseMoney : MonoBehaviour
{
    [SerializeField] public int value = 5; //the value of this pickup
    [SerializeField] private float rotationSpeed = 80; //how fast the object rotates on the ground

    [SerializeField] private GameObject collectableObj; //this is the gameobject that reacts with the player's hitbox to be collected
    private Rigidbody rb;

    private Animator anim;

    private bool collected = false; //has this object been collected yet

    private float activateTime = 0.55f; //how long until this object starts reacting to the player (can be collected, moves towards player)
    private float timeTemp = 0;

    [SerializeField] private float gravity = 14; //a manual gravity value that can be used to fine tune how the money feels

    public bool activated = false; //is this object active yet

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //rotate the money in place
        transform.Rotate(Vector3.up * (Time.deltaTime * rotationSpeed));

        //count up until this object activates
        if (timeTemp < activateTime)
        {
            timeTemp += Time.deltaTime;
        }
        else
        {
            activated = true;
        }
    }

    private void FixedUpdate()
    {
        //add our gravity to the rigidbody
        rb.AddForce(Vector3.down * gravity, ForceMode.Force);
    }

    public void Collect()
    {
        
        if (!collected)
        {
            //set the object that the player can collect to inactive
            collectableObj.SetActive(false);

            //freeze the rigidbody
            rb.linearVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePosition;

            //this object is collected
            collected = true;

            OnCollect();
        }
     
    }

    private void OnCollect()
    {
        //set animator and destroy this object
        anim.SetTrigger("OnCollect");
        Destroy(this.gameObject, 0.5f);
    }
}
