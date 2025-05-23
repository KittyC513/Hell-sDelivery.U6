using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerItemControl : MonoBehaviour
{
    public ItemHandler activeItem;

    [SerializeField] public PlayerStateMachine playerStateMachine;
    [SerializeField] private PlayerInputDetection inputDetection;



    private void Update()
    {
        //if we input the button to use an item and we have an active item try to invoke it
        if (inputDetection.crouchPressed && activeItem != null && activeItem.name != "Bomb Item")
        {
            UseItem();
        }

        if(activeItem != null)
        {
            //Bomb 
            if (activeItem.name == "Bomb Item")
                UseBomb();
            //Detonator
            else if (activeItem.name == "Detonator Item")
                UseDetonator();
        } 

        if(activeItem != null && inputDetection.IsDropActiveItem())
        {
            UnequipItem();
        }

    }
    #region Bomb Event
    private void UseBomb()
    {
        //if (inputDetection.lockPressed)
        //{
        //    activeItem.gameObject.GetComponent<BombItem>().isHeldBomb = true;

        //}
        //else
        //{
        //    activeItem.gameObject.GetComponent<BombItem>().isHeldBomb = false;
        //}

        //if (inputDetection.crouchPressed && activeItem.gameObject.GetComponent<BombItem>().isHeldBomb)
        //{
        //    UseItem();
        //    activeItem.gameObject.GetComponent<BombItem>().isSpawned = false;
        //}

        if (inputDetection.crouchPressed)
        {
            UseItem();
            activeItem.gameObject.GetComponent<BombItem>().isSpawned = false;
        }

        if (!inputDetection.crouchPressed)
        {
            activeItem.gameObject.GetComponent<BombItem>().canStartTimer = true;
        }
    }
    #endregion

    #region Detonator Event
    private void UseDetonator()
    {
        if (inputDetection.crouchPressed && activeItem.gameObject.GetComponent<DetonatorItem>().bombItem.bombsList.Count > 0)
        {
            UseItem();
            Debug.Log("bomb list :" + activeItem.gameObject.GetComponent<DetonatorItem>().bombItem.bombsList.Count);
            activeItem.gameObject.GetComponent<DetonatorItem>().canStartTimer = false;
            //activeItem.gameObject.GetComponent<DetonatorItem>().bombItem.bombsList.Clear();
        }
        if (!inputDetection.crouchPressed)
        {
            activeItem.gameObject.GetComponent<DetonatorItem>().canStartTimer = true;
        }
    }
    #endregion


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
