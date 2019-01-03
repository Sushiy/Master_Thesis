using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    HashSet<Item> items;

    public HashSet<Item> Items
    {
        get { return items; }
    }

    public Inventory()
    {
        items = new HashSet<Item>();
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }    
}
