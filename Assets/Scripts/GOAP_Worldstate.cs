using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldStateKey
{
    bHasAxe = 0,
    bHasLog = 1,
    bHasWood = 2,
    bAttackingTarget = 3,
    bWeaponEquipped = 4,
    bHasIron = 5
}

[System.Serializable]
public struct GOAP_Worldstate : System.IEquatable<GOAP_Worldstate>
{
    //from the above list
    public WorldStateKey key;

    //Can be null
    public GameObject target;
    //This is unsafe, each WorldstateKey signifies which type of value it should receive, but this can't be properly checked.
    public bool value;

    public GOAP_Worldstate(WorldStateKey key, bool value, GameObject target)
    {
        this.key = key;
        this.value = value;
        this.target = target;
    }

    public bool Equals(GOAP_Worldstate other)
    {
        return Equals(other, this);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        GOAP_Worldstate objectToCompareWith = (GOAP_Worldstate)obj;
        return objectToCompareWith.key == key && objectToCompareWith.value == value;
    }

    public override int GetHashCode()
    {
        int calculation = (int)key;
        return calculation;
    }

    public static bool operator ==(GOAP_Worldstate state1, GOAP_Worldstate state2)
    {
        if (object.ReferenceEquals(state1, state2)) return true;
        if (object.ReferenceEquals(state1, null)) return false;
        if (object.ReferenceEquals(state2, null)) return false;

        return state1.Equals(state2);
    }

    public static bool operator !=(GOAP_Worldstate state1, GOAP_Worldstate state2)
    {
        if (object.ReferenceEquals(state1, state2)) return false;
        if (object.ReferenceEquals(state1, null)) return true;
        if (object.ReferenceEquals(state2, null)) return true;

        return !state1.Equals(state2);
    }

    public override string ToString()
    {
        return key.ToString() + ":" + value.ToString();
    }
}

