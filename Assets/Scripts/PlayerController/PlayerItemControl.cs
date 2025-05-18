using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemControl : MonoBehaviour
{
    public ItemHandler activeItem;

    [SerializeField] public PlayerStateMachine playerStateMachine;
    [SerializeField] private PlayerInputDetection inputDetection;



    private void Update()
    {
        //if we input the button to use an item and we have an active item try to invoke it
        if (inputDetection.crouchPressed && activeItem != null)
        {
            UseItem();
            //if the item is bomb, it will try different method to invoke the event
            activeItem.isBombTriggered = true;
        }

        //To invoke and continue the 'Bomb' event until it reaches its desired destination
        if(activeItem != null && activeItem.isBombTriggered)
            UseItem();


    }

    //invoke the current item's on trigger event
    private void UseItem()
    {
        //if the active item's trigger delegate is not null invoke it on item use
        activeItem.onItemTrigger?.Invoke();
    }

    //equip an item
    private void EquipItem(ItemHandler item)
    {
        activeItem = item;
        item.EquipItem(this);
    }

    //unequip an item
    private void UnequipItem()
    {
        activeItem.UnequipItem();
        activeItem = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if (activeItem == null)
            {
                EquipItem(other.GetComponent<ItemHandler>());
            }
        }
    }
}
