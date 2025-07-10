using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    public List<ItemBase> bag;

    public PlayerInputDetection inputDetection;

    public Transform equipPoint;
    public Transform bagPoint;

    public float swapCooldown = 0.3f;
    public float swapTimer = 0f;

    private void Awake()
    {
        bag = new List<ItemBase>();
    }
    public void AddItem(ItemBase item)
    {
        bag.Add(item);
    }

    public void RemoveItem(ItemBase item)
    {
        bag.Remove(item);
    }

    public void FixedUpdate()
    {
        EquipItem();
    }

    public void EquipItem()
    {
        //1.check if bag has items
        if (bag.Count > 0)
        {
            //2. check if the first item in the bag is on use
            if (bag[0].isOnUse)
            {
                //3. if the item is on use and we press the swap button, swap it to the bag point
                if (inputDetection.swapItemPressed_left && swapTimer > swapCooldown)
                {
                    //4. reset the swap timer and set the item to not on use
                    swapTimer = 0;
                    bag[0].isOnUse = false;
                }
                //5. set the item's position to the equip point
                bag[0].transform.position = equipPoint.position;
            }
            else
            {
                //6. if the item is not on use and we press the swap button, swap it to the equip point
                if (inputDetection.swapItemPressed_left && swapTimer > swapCooldown)
                {
                    //7. reset the swap timer and set the item to on use
                    swapTimer = 0;
                    bag[0].isOnUse = true;
                }
                //8. set the item's position to the bag point
                bag[0].transform.position = bagPoint.position;
            }

            //9. update the swap timer
            if (swapTimer <= swapCooldown)
            {
                if(swapTimer >= 1f)
                {
                    swapTimer = 1f;
                }
                else
                {
                    // If the swap button is not pressed, start the timer
                    if (!inputDetection.swapItemPressed_left)
                    {
                        swapTimer += Time.deltaTime;
                    }
                }
            }
        }
    }
}
