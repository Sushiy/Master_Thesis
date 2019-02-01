using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldStateKey
{
    eHasItem = 0,
    bAttackingTarget = 1,
    bHasWorked,
    bIsHungry,
    bIsHealthy,
    bIsTired,
    bHasStockpiledRessources
}

[System.Serializable]
public struct GOAP_Worldstate : System.IEquatable<GOAP_Worldstate>
{
    //from the above list
    public WorldStateKey key;

    //Can be null
    public IActionTarget target;

    public int value;

    public GOAP_Worldstate(WorldStateKey key, bool value, IActionTarget target = null) : this(key, value == true ? 1 : 0, target)
    {
    }

    public GOAP_Worldstate(WorldStateKey key, int value, IActionTarget target = null)
    {
        this.key = key;
        this.value = value;
        this.target = target;
    }

    public bool Equals(GOAP_Worldstate other)
    {
        if(IsUniqueState())
            return other.key == this.key;
        else
            return other.key == this.key && other.value == this.value;

    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        GOAP_Worldstate objectToCompareWith = (GOAP_Worldstate)obj;
        return Equals(objectToCompareWith);
    }

    /// <summary>
    /// For UniqueStates only the key is used as a hashcode, meaning the same hash will be returned for any value.
    /// This is not the case for nonUnique States like eHasItem, which can exist multiple times with different values.
    /// /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int calculation = (int)key.GetHashCode() * (IsUniqueState() ? 1 : value.GetHashCode());
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

    public bool IsUniqueState()
    {
        //Add all nonunique keys here
        if(key == WorldStateKey.eHasItem)
        {
            return false;
        }
        return true;
    }
}

