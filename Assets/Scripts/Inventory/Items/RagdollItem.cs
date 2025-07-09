using UnityEngine;

public class RagdollItem : MonoBehaviour
{
    private ItemHandler iHandler;
    public Collider worldCollider;

    public Vector3 offset;

    private void Awake()
    {
        iHandler = GetComponent<ItemHandler>();
        if (iHandler == null) Debug.Log(this.ToString() + " needs an itemHandler attached on the gameObject " + this.gameObject.name);
    }
    private void Start()
    {
        iHandler.onItemTrigger += StartRagdoll;
    }

    private void OnDisable()
    {
        iHandler.onItemTrigger -= StartRagdoll;
    }

    private void StartRagdoll()
    {
        //change the player's active state to the ragdoll state
        
        iHandler.OverrideStateMachine(PlayerStateMachine.PlayerStates.ragdoll);
        
    }

    private void FixedUpdate()
    {
        if(iHandler.equipped)
        {
            if(worldCollider.enabled == true)
                worldCollider.enabled = false;

            if (Vector3.Distance(this.transform.position, iHandler.iControl.transform.position + offset) < 0.1f)
                this.transform.SetParent(iHandler.iControl.transform);
            else
                this.transform.position = iHandler.iControl.transform.position + offset;
        }
                     
        if (!iHandler.equipped && (worldCollider.enabled == false || iHandler.rb.useGravity == false))
        {
            worldCollider.enabled = true;
            iHandler.rb.useGravity = true;
        }

    }


}
