using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Skills
{
    WoodCutting = 0,
    Smithing = 1,
    Fighting = 2
}

[System.Serializable]
public class GOAP_Skill : System.IEquatable<GOAP_Skill>
{
    public Skills id;
    public int level;

    public GOAP_Skill(Skills id, int level)
    {
        this.id = id;
        this.level = level;
    }

    public bool Equals(GOAP_Skill other)
    {
        return Equals(other, this);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        GOAP_Skill objectToCompareWith = (GOAP_Skill)obj;
        return objectToCompareWith.id == id;
    }

    public override int GetHashCode()
    {
        int calculation = id.GetHashCode();
        return calculation;
    }

    public static bool operator ==(GOAP_Skill obj1, GOAP_Skill obj2)
    {
        if (object.ReferenceEquals(obj1, obj2)) return true;
        if (object.ReferenceEquals(obj1, null)) return false;
        if (object.ReferenceEquals(obj2, null)) return false;

        return obj1.Equals(obj2);
    }

    public static bool operator !=(GOAP_Skill obj1, GOAP_Skill obj2)
    {
        if (object.ReferenceEquals(obj1, obj2)) return false;
        if (object.ReferenceEquals(obj1, null)) return true;
        if (object.ReferenceEquals(obj2, null)) return true;

        return !obj1.Equals(obj2);
    }
}