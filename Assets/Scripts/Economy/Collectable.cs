using UnityEngine;
using UnityEngine.Events;

//this object can be collected by the player
public class Collectable : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private GameObject collectableObj; //this is the gameobject that reacts with the player's hitbox to be collected
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rb;

    [Space, Header("Settings")]
    [SerializeField] public bool enableMagnetism = false;
    [SerializeField] private bool shouldRotate = true;
    [SerializeField] private float rotationSpeed = 80;
    [SerializeField] private float activateTime = 0.55f; //how long until this object starts reacting to the player (can be collected, moves towards player)

    public delegate void OnCollect(GameObject collector, Collectable collectable);
    public OnCollect onCollect;

    private bool collected = false; //has this object been collected yet
    private float timeTemp = 0;
    [HideInInspector] public bool activated = false; //is this object active yet

    private void Start()
    {
        if (enableMagnetism && rb == null)
        {
            Debug.Log("A rigidbody is required for magnetism, please attach one");
            enableMagnetism = false;
        }
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
        else if (!activated)
        {
            activated = true;
        }
    }

    public void Collect(GameObject collector)
    {
        if (!collected)
        {
            //set the object that the player can collect to inactive
            collectableObj.SetActive(false);

            if (rb != null)
            {
                //freeze the rigidbody
                rb.linearVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezePosition;
            }

            //this object is collected
            collected = true;

            //trigger collect event
            onCollect?.Invoke(collector, this);

            CollectSequence();
        }
    }

    private void CollectSequence()
    {
        //set animator and destroy this object
        if (anim != null) anim.SetTrigger("OnCollect");
        
        Destroy(this.gameObject, 0.5f);
    }
}
