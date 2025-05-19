using UnityEngine;

public class BombMovement : MonoBehaviour
{
    public PlayerLockOn playerLockOn;
    public Transform targetPos;
    public float dropSpeed;

    public float radius;
    public Collider[] colliders;
    public float explosionForce;

    public bool isTriggered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Bomb Pos" + this.transform.position);
        playerLockOn = this.transform.GetComponentInParent<PlayerLockOn>();
        targetPos = playerLockOn.lockTarget.transform;
        print("Target Position" + targetPos.position);
        this.transform.parent = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move toward target all the time
        if (!isTriggered)
        {
            MoveTowardTarget(targetPos);
        }

    }


    void MoveTowardTarget(Transform targetPos)
    {
        if (Vector3.Distance(this.transform.position, targetPos.position) > 0.1f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos.position, Time.deltaTime * dropSpeed);

            //print("bomb is on the target");
        }
    }

    [ContextMenu("Boom!")]
    public void ApplyExplosionForce()
    {
        //Detect the explosion area, it's a sphere detector, set LayerMask that to be affected
        isTriggered = true;
        colliders = Physics.OverlapSphere(this.transform.position, radius, 1 << LayerMask.NameToLayer("Lockable"));
        Debug.Log(colliders.Length + "_enemy/enemies in the explosion range");

        if (colliders.Length > 0) 
        {
            //apply the force
            for (int i = 0; i < colliders.Length; i++) 
            {
                colliders[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, this.transform.position, radius);
            }

        }
        //Destroy after the certain amount of time
        Destroy(this.gameObject, 0.3f);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }

}
