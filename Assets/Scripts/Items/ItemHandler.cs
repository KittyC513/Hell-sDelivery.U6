using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

//has functionality that all items will want access to
//such as being able to be picked up and dropped as well as any interactions with the player controller
public class ItemHandler : MonoBehaviour
{
    [HideInInspector]
    public bool equipped = false;

    [Header ("Non Equipped Variables")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    //equipped variables
    [HideInInspector]
    public PlayerItemControl iControl;

    //any functionality that needs to happen while equipped can subscribe to this event
    public delegate void OnItemTrigger();
    public OnItemTrigger onItemTrigger;

    private Collider triggerCollider;

    [HideInInspector]
    public Rigidbody rb;





    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }
    //player equips this item
    public void EquipItem(PlayerItemControl playerItemControl)
    {
        StopCoroutine(DropCoroutine());
        Debug.Log("Equipped");
        iControl = playerItemControl;
        equipped = true;

        //Freeze rotation and reset rotation 
        rotationSpeed = 0;
        this.transform.rotation = iControl.transform.rotation;

        triggerCollider.enabled = false;
        rb.useGravity = false;

    }

    //player unequips this item
    public void UnequipItem()
    {
        UnfreezeStateMachine();
        //iControl = null;
        //equipped = false;
        //rotationSpeed = 100;
        //triggerCollider.enabled = true;

        StartCoroutine(DropCoroutine());
    }

    #region Drop Coroutine
    IEnumerator DropCoroutine()
    {
        Debug.Log("Unequipt");
        equipped = false;
        this.transform.parent = null;
        rotationSpeed = 100;
        iControl = null;
        yield return new WaitForSeconds(1f);
        triggerCollider.enabled = true;

    }
    #endregion

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
