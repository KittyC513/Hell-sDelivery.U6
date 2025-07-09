using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    public List<ItemBase> bag;

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
}
