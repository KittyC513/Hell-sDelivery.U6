using UnityEngine;

public class BombItem : MonoBehaviour
{
    private ItemHandler iHandler;
    private Transform parentObj;
    private PlayerLockOn playerLockOn;

    private Transform targetPos;
    public float dropSpeed = 5f;

    public Collider worldCollider;
    private Rigidbody rb;





    private void Awake()
    {
        iHandler = GetComponent<ItemHandler>();
        if (iHandler == null) Debug.Log(this.ToString() + " needs an itemHandler attached on the gameObject " + this.gameObject.name); 
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        iHandler.onItemTrigger += ThrowTowardTarget;
    }

    private void OnDisable()
    {
        iHandler.onItemTrigger -= ThrowTowardTarget;
    }

    private void ThrowTowardTarget()
    {
        print("Throw");
        parentObj = this.transform.parent.Find("Player").transform;
        playerLockOn = parentObj.GetComponentInParent<PlayerLockOn>();
        worldCollider.gameObject.SetActive(false);
        if (playerLockOn.lockTarget != null)
        {
            targetPos = playerLockOn.lockTarget.transform;
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos.position, Time.deltaTime * dropSpeed);

            if (Vector3.Distance(this.transform.position, targetPos.position) < 0.1f) 
            {
                iHandler.isBombTriggered = false;
            }
            return;
        }       
    }



}
