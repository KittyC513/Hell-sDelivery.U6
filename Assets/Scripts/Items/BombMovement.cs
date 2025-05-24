using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.ScrollRect;

public class BombMovement : MonoBehaviour
{
    public PlayerLockOn playerLockOn;
    public Transform targetPos;
    public float dropSpeed;

    public float radius;
    public Collider[] colliders_e;
    public Collider[] colliders_p;
    public float explosionForce_e;
    public float explosionForce_pH;
    public float explosionForce_pV;
    public float upwardsModifier_e;

    public bool isTriggered = false;

    //Drop variables
    //parabolic arc movement Method
    public float maxDistance;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float height = 5f;
    public float duration = 1f;
    private float time = 0f;
    public float dropForce;
    public float forceScale = 1f;
    public float offsetY = 0f;


    //ground check
    public bool isOnGround = false;


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
            else
            {
                if (!isOnGround)
                    ThrowBombWithoutTarget();

            }


        }

    }

    #region Bomb Movement
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
            if (!isOnGround)
                MovementMethod();

        }
        #endregion

    }

    /// <summary>
    /// When not target is locked on
    /// </summary>
    void MovementMethod()
    {
        Debug.Log("the target is over max distance of throwing");
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
    #endregion

    #region when not target is locked on, throwing a bomb

    void ThrowBombWithoutTarget()
    {
        Debug.Log("Not target is locked on");
        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / duration);
        //gain end point, the pos of end point can be modified by dropping force
        endPoint = playerLockOn.transform.forward * dropForce * forceScale + startPoint;
        endPoint.y = offsetY;
        // Linear interpolation for X and Z
        Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);
        // Parabolic height using a simple formula: 4h * t * (1 - t)
        float parabola = 4 * height * t * (1 - t);
        currentPos.y = Mathf.Lerp(startPoint.y, endPoint.y, t) + parabola;
        
        this. transform.position = currentPos;
    }
    #endregion


    #region Explosion Function - apply force to different objects 
    public void ApplyExplosionForce()
    {
        isTriggered = true;

        //Detect the explosion area, it's a sphere detector, set LayerMask that to be affected
        colliders_e = Physics.OverlapSphere(this.transform.position, radius, 1 << LayerMask.NameToLayer("Lockable") | 1 << LayerMask.NameToLayer("Enemy"));
        colliders_p = Physics.OverlapSphere(this.transform.position, radius, 1 << LayerMask.NameToLayer("Player1") | 1 << LayerMask.NameToLayer("Player2"));

        Debug.Log(colliders_e.Length + "_enemy/enemies in the explosion range");
        Debug.Log(colliders_p.Length + "player/players in the explosion range");
        #region Enemy type
        if (colliders_e.Length > 0)
        {
            for (int i = 0; i < colliders_e.Length; i++)
            {
                colliders_e[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce_e, this.transform.position, radius, upwardsModifier_e);
                colliders_e[i].GetComponent<EnemyHealth>().TakeDamage(2);
            }

        }
        #endregion

        #region Player type
        if (colliders_p.Length > 0)
        {
            Debug.Log("Player : "+ colliders_p.Length);
            for (int i = 0; i < colliders_p.Length; i++)
            {
                Debug.Log("Player : " + colliders_p[i].name);
                // gain the dirction between bomb and player
                Vector3 dir = (colliders_p[i].transform.position - this.transform.position).normalized;
                colliders_p[i].GetComponent<Rigidbody>().AddForce(dir * explosionForce_pH + Vector3.up * explosionForce_pV, ForceMode.Impulse);
                colliders_p[i].GetComponent<PlayerStateMachine>().OverrideState(PlayerStateMachine.PlayerStates.airborne);
                //colliders_p[i].GetComponent<PlayerController>().fallAccelScale = 0.2f;
            }
        }

        #endregion
        //Destroy after the certain amount of time
        Destroy(this.gameObject, 0.2f);

    }
    #endregion


    #region DrawGizoms Function
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
    #endregion


    #region GrounCheck
    //Ground Check
    private bool GroundCheck()
    {
        //Ground check method
        return false;
    }

    
    #endregion


}
