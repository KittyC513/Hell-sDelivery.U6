using UnityEngine;

public class LooseMoney : MonoBehaviour
{
    [SerializeField] public int value = 5;
    [SerializeField] private float rotationSpeed = 80;

    [SerializeField] private GameObject collectableObj;
    private Rigidbody rb;

    private Animator anim;

    private bool collected = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //rotate the money in place
        transform.Rotate(Vector3.up * (Time.deltaTime * rotationSpeed));
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
