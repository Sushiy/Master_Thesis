using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldStateKey
{
    eHasItem = 0,
    bAttackingTarget = 1,
    bHasCheckedQuestboard,
    bHasEaten,
    bIsHealthy,
    bHasSlept,
    bHasSocialised,
    bHasStockpiledRessources,
    bWasFieldTended,
    bIsWheatRipe,
    bIsMushroomAvailable,
    bIsDeadWoodAvailable,
    bIsIronAvailable
}

public enum WorldStateType
{
    UNIQUE = 0,
    INDEXED,
    TARGETED
}

[System.Serializable]
public class GOAP_Worldstate : System.IEquatable<GOAP_Worldstate>
{
    //from the above list
    public WorldStateKey key;

    //Can be null
    public IActionTarget target;

    public WorldStateType type { private set; get; }

    public int value;

    float forgetTime = 0.0f; //How long it takes for the NPC to forget this state (0 means he wont forget at all)
    float forgetAlpha = 0f;

    public GOAP_Worldstate(WorldStateKey key, bool value, IActionTarget target = null) : this(key, value == true ? 1 : 0, target)
    {
    }

    public GOAP_Worldstate(WorldStateKey key, int value, IActionTarget target = null)
    {
        this.key = key;
        this.value = value;
        this.target = target;
        type = DetermineType();
        forgetTime = DetermineForgetTime();
    }

    public bool Forget(float deltaTime)
    {
        if(forgetTime > 0.0f)
        {
            forgetAlpha += deltaTime / forgetTime;
            if (forgetAlpha > 1)
                return true;
        }
        return false;
    }

    public bool Equals(GOAP_Worldstate other)
    {
        if(this.type == WorldStateType.UNIQUE)
            return other.key == this.key;
        else if(this.type == WorldStateType.INDEXED)
            return other.key == this.key && other.value == this.value;
        else 
            return other.key == this.key && other.target == this.target;

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
        int calculation = (int)key.GetHashCode() * (this.type == WorldStateType.UNIQUE ? 1 : value.GetHashCode());
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
        if(key == WorldStateKey.eHasItem)
            return key.ToString() + ":" + ((ItemType)value).ToString();
        else
            return key.ToString();
    }

    private WorldStateType DetermineType()
    {
        switch(key)
        {
            case WorldStateKey.eHasItem:
                return WorldStateType.INDEXED;
            //case WorldStateKey.bWasFieldTended:
            //case WorldStateKey.bIsWheatRipe:
            //case WorldStateKey.bIsMushroomAvailable:
            //case WorldStateKey.bIsIronAvailable:
            //case WorldStateKey.bIsDeadWoodAvailable:
            //    return WorldStateType.TARGETED;
            default:
                return WorldStateType.UNIQUE;
        }
    }

    private float DetermineForgetTime()
    {
        switch (key)
        {
            case WorldStateKey.bWasFieldTended:
            case WorldStateKey.bIsWheatRipe:
                return Field_GOAT.GROWTHTIME;
            case WorldStateKey.bIsMushroomAvailable:
            case WorldStateKey.bIsIronAvailable:
            case WorldStateKey.bIsDeadWoodAvailable:
                return 40f;
            case WorldStateKey.bHasCheckedQuestboard:
                return 5f;
            default:
                return 0f;
        }
    }
}


