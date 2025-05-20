using System.Net;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.ScrollRect;

public enum E_MovementType
{
    Method1,
    Method2,
}
public class BombMovement : MonoBehaviour
{
    public PlayerLockOn playerLockOn;
    public Transform targetPos;
    public float dropSpeed;

    public float radius;
    public Collider[] colliders;
    public float explosionForce;

    public bool isTriggered = false;

    //Drop variables
    //parabolic arc movement Method
    public float maxDistance;
    private Vector3 startPoint;
    private Vector3 endPoint;
    public float height = 5f;
    public float duration = 1f;
    private float time = 0f;
    public float dropForce;
    public float offsetY = 0f;

    //ground check
    private bool isOnGround = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPoint = this.transform.position;
        //print("Bomb Pos" + this.transform.position);
        playerLockOn = this.transform.GetComponentInParent<PlayerLockOn>();
        if(playerLockOn.lockTarget != null)
            targetPos = playerLockOn.lockTarget.transform;

        this.transform.parent = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move toward target all the time
        if (!isTriggered)
        {
            //When a target is locked on
            if(targetPos != null)
            {
                MoveTowardTarget(targetPos);
            }



            //No target is locked on
        }

    }


    void MoveTowardTarget(Transform targetPos)
    {
        #region Bomb will move toward a target when it's within max dropping range

        if (Vector3.Distance(this.transform.position, targetPos.position) <= maxDistance)
        {

            if (Vector3.Distance(this.transform.position, targetPos.position) < 0.1f)
            {

                this.transform.position = Vector3.Lerp(this.transform.position, targetPos.position, Time.deltaTime * dropSpeed);

                //print("bomb is sticked to the target");
            }
            else
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                //gain end point, the pos of end point can be modified by dropping force
                endPoint = targetPos.position;
                // Linear interpolation for X and Z
                Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);
                // Parabolic height using a simple formula: 4h * t * (1 - t)
                float parabola = 2 * height * t * (1 - t);
                currentPos.y = Mathf.Lerp(startPoint.y, endPoint.y, t) + parabola;

                transform.position = currentPos;

            }
        }
        #endregion
        #region the target is over max distance of throwing
        else
        {
            if(!isOnGround)
                MovementMethod();

        }
        #endregion

    }


    void MovementMethod()
    {
        
        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / duration);
        //gain end point, the pos of end point can be modified by dropping force
        endPoint = playerLockOn.transform.forward * dropForce + startPoint;
        endPoint.y = offsetY;
        // Linear interpolation for X and Z
        Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);
        // Parabolic height using a simple formula: 4h * t * (1 - t)
        float parabola = 4 * height * t * (1 - t);
        currentPos.y = Mathf.Lerp(startPoint.y, endPoint.y, t) + parabola;

        transform.position = currentPos;
    }


    [ContextMenu("Boom!")]
    public void ApplyExplosionForce()
    {
        isTriggered = true;
        //Detect the explosion area, it's a sphere detector, set LayerMask that to be affected
        colliders = Physics.OverlapSphere(this.transform.position, radius, 1 << LayerMask.NameToLayer("Lockable"));
        Debug.Log(colliders.Length + "_enemy/enemies in the explosion range");
        if (colliders.Length > 0)
        {
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

    //Ground Check
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
            isOnGround = true;
    }

}
