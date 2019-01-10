using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Inventory
{
    Dictionary<ItemType, int> items;

    public Dictionary<ItemType, int> Items
    {
        get { return items; }
    }

    public Inventory()
    {
        items = new Dictionary<ItemType, int>();
    }

    //Returns true if the item was new or false if it wasnt
    public bool AddItem(ItemType item, int count = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] += count;
            return false;
        }
        else
        {
            items.Add(item, count);
            return true;
        }
    }

    public bool RemoveItem(ItemType item, int count = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= count;
            if (items[item] <= 0)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
        return true;
    }
}
