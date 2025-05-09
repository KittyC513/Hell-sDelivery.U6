using UnityEngine;

public class RagdollItem : MonoBehaviour
{
    private ItemHandler iHandler;

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

}
