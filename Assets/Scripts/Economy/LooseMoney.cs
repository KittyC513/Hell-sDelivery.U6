using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class LooseMoney : MonoBehaviour
{
    [SerializeField] public int value = 5;
    [SerializeField] private float rotationSpeed = 80;

    [SerializeField] private GameObject collectableObj;
    private Rigidbody rb;

    private Animator anim;

    private bool collected = false;

    private float activateTime = 0.55f;
    private float timeTemp = 0;

    [SerializeField] private float gravity = 2;

    public bool activated = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //rotate the money in place
        transform.Rotate(Vector3.up * (Time.deltaTime * rotationSpeed));

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
        anim.SetTrigger("OnCollect");
        Destroy(this.gameObject, 0.5f);
    }
}
