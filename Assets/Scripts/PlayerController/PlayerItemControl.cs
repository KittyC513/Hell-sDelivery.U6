using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemControl : MonoBehaviour
{
    private ItemHandler activeItem;

    [SerializeField] public PlayerStateMachine playerStateMachine;

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
}
