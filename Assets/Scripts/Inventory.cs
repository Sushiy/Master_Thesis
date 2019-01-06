using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    //Resources are in 0x0..
    Wood = 0x00,
    Log = 0x01,
    Iron = 0x02,
    Wheat = 0x03,
    Flour = 0x04,
    Water = 0x05,
    Stone = 0x06,

    //Tools are in 0x10..
    Axe = 0x101,
    Hammer = 0x102,

    //Items are in 0x100..
    Bread = 0x1001,
}

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
        if(items.ContainsKey(item))
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

    //Returns true if the item was completely removed or false if it wasnt
    public bool RemoveItem(ItemType item, int count = 1)
    {
        if(items.ContainsKey(item))
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
