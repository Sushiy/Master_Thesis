using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    Dictionary<Item, int> items;

    public Dictionary<Item, int> Items
    {
        get { return items; }
    }

    public Inventory()
    {
        items = new Dictionary<Item, int>();
    }

    //Returns true if the item was new or false if it wasnt
    public bool AddItem(Item item, int count = 1)
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
    public bool RemoveItem(Item item, int count = 1)
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
