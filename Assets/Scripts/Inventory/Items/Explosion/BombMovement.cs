using NUnit.Framework;
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
    private Collider[] colliders_o;
    public float explosionForce_e;
    public float explosionUpForce_e;

    public float explosionForce_pH;
    public float explosionForce_pV;
    public float upwardsModifier_e;

    public bool isTriggered = false;
    private bool isThrew = false;

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
    public float groundCheckDist = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPoint = this.transform.position;
        colliders_o = new Collider[5];
        //print("Bomb Pos" + this.transform.position);

        //if(playerLockOn.lockTarget != null)
        //    targetPos = playerLockOn.lockTarget.transform;
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
                //MoveTowardTarget(targetPos);
            }
            //No target is locked on
            else
            {
                //if (!GroundCheck())
                    //ThrowBombWithoutTarget();

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
            if (!GroundCheck())
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
        if (!isThrew)
        {
            endPoint = playerLockOn.transform.forward * dropForce + startPoint;
            isThrew = true;
        }
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
        if (!isThrew)
        {
            endPoint = playerLockOn.transform.forward * dropForce * forceScale + startPoint;
            isThrew = true;
        }
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
        colliders_p = Physics.OverlapSphere(this.transform.position, radius, 1 << LayerMask.NameToLayer("Player1") | 1 << LayerMask.NameToLayer("Player2") | 1 << LayerMask.NameToLayer("Invisible_Player1") | 1 << LayerMask.NameToLayer("Invisible_Player2"));
        
        int o = Physics.OverlapSphereNonAlloc(this.transform.position, radius, colliders_o, 1 << LayerMask.NameToLayer("Explodable"));
        
        if (o > 0)
        {
            for (int i = 0; i < o; i++)
            {
                if (colliders_o[i].CompareTag("Switch"))
                {
                    colliders_o[i].GetComponent<BombSwitch>().TriggerSwitch();
                }
            }
        }

        Debug.Log(colliders_e.Length + "_enemy/enemies in the explosion range");
        Debug.Log(colliders_p.Length + "player/players in the explosion range");
        #region Enemy type
        if (colliders_e.Length > 0)
        {
            for (int i = 0; i < colliders_e.Length; i++)
            {
                Vector3 dir = (colliders_e[i].transform.position - this.transform.position).normalized;

                print(dir);

                if (colliders_e[i].name.Contains("Hell Hound"))
                    colliders_e[i].GetComponent<HellHoundBase>().StartKnockback(dir * explosionForce_e + Vector3.up * explosionUpForce_e);
                if (colliders_e[i].name.Contains("Thief"))
                    colliders_e[i].GetComponent<ThiefBase>().StartKnockback(dir * explosionForce_e);
                
                colliders_e[i].GetComponent<Health>().TakeDamage(2);
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
                colliders_p[i].GetComponent<PlayerController>().GainExplodedForce(dir);
                //colliders_p[i].GetComponent<Rigidbody>().AddForce(dir * explosionForce_pH + Vector3.up * explosionForce_pV, ForceMode.Impulse);
                colliders_p[i].GetComponent<PlayerStateMachine>().OverrideState(PlayerStateMachine.PlayerStates.freeFall);
                colliders_p[i].GetComponent<PlayerController>().fallAccelScale = 0.5f;
            }
        }

        #endregion
        //Destroy after the certain amount of time
        Destroy(this.gameObject, 0.5f);

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

        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, groundCheckDist, 1 << LayerMask.NameToLayer("Ground")))
        {
            Debug.Log("Bomb is grounded");
            StickToSurface(hit.transform);
            

            return true;
        }
        return false;

    }

    private void StickToSurface(Transform obj)
    {
        if (this.transform.parent != obj)
        {
            this.transform.parent = obj.transform;
        }
    }
    #endregion


}
