using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//has functionality that all items will want access to
//such as being able to be picked up and dropped as well as any interactions with the player controller
public class ItemHandler : MonoBehaviour
{
    private bool equipped = false;

    [Header ("Non Equipped Variables")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    //equipped variables
    private PlayerItemControl iControl;

    //any functionality that needs to happen while equipped can subscribe to this event
    public delegate void OnItemTrigger();
    public OnItemTrigger onItemTrigger;


    //player equips this item
    public void EquipItem(PlayerItemControl playerItemControl)
    {
        Debug.Log("Equipped");
        iControl = playerItemControl;
        equipped = true;
    }

    //player unequips this item
    public void UnequipItem()
    {
        UnfreezeStateMachine();
        iControl = null;
        equipped = false;
    }

    //this function grabs the player state machine and freezes the state machine
    public void FreezeStateMachine()
    {
        if (equipped) iControl.playerStateMachine.FreezeStateMachine();
    }

    //this function grabs the player state machine and unfreezes the state machine
    public void UnfreezeStateMachine()
    {
        if (equipped) iControl.playerStateMachine.UnFreezeStateMachine();
    }

    public void OverrideStateMachine(PlayerStateMachine.PlayerStates targetState)
    {
        iControl.playerStateMachine.OverrideState(targetState);
    }
    private void Update()
    {
        Rotate();
    }

    //simply rotate when not equipped
    private void Rotate()
    {
        transform.Rotate(rotationAxis * (Time.deltaTime * rotationSpeed));
    }
}
