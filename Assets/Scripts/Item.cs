using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemIds
{
    //Resources are in 0x0..
    Wood = 0x00,
    Log = 0x01,
    Iron = 0x02,
    Wheat = 0x03,
    Flour = 0x04,
    Water = 0x05,
    Stone = 0x06,
    //Tools are in 0xA..
    Axe = 0xA1,
    Hammer = 0xA2,
    
}

[System.Serializable]
public struct Item : System.IEquatable<Item>
{
    string name;
    public string Name
    {
        get
        {
            return name;
        }
    }

    ItemIds id;
    public ItemIds Id
    {
        get
        {
            return id;
        }
    }

    public Item(ItemIds id) : this(id.ToString(), id)
    {
    }

    public Item(string name, ItemIds id)
    {
        this.id = id;
        this.name = name;
    }

    public bool Equals(Item other)
    {
        return this.id.Equals(other.Id) && this.name.Equals(other.Name);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        GOAP_Worldstate objectToCompareWith = (GOAP_Worldstate)obj;
        return this.Equals(objectToCompareWith);
    }

    public override int GetHashCode()
    {
        int calculation = (int)id + name.GetHashCode();
        return calculation;
    }
}
