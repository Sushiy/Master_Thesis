using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_GOAP_Worldstate : List<GOAP_Worldstate>
{
    public List_GOAP_Worldstate():base()
    {

    }

    /// <summary>
    /// This is a deepCopy constructor
    /// </summary>
    /// <param name="list"></param>
    public List_GOAP_Worldstate(List_GOAP_Worldstate list) : this(list.ToArray())
    {
    }
    /// <summary>
    /// This is a deepCopy constructor
    /// </summary>
    /// <param name="list"></param>
    public List_GOAP_Worldstate(List<GOAP_Worldstate> list):this(list.ToArray())
    {
    }
    /// <summary>
    /// This is a deepCopy constructor
    /// </summary>
    /// <param name="list"></param>
    public List_GOAP_Worldstate(GOAP_Worldstate[] array) : this()
    {
        for (int i = 0; i < array.Length; i++)
        {
            Add(array[i]);
        }
    }

    public bool ContainsKey(GOAP_Worldstate state)
    {
        for(int i = 0; i < Count; i++)
        {
            if(this[i].key == state.key && (state.type == WorldStateType.UNIQUE || this[i].value == state.value))
            {
                return true;
            }
        }

        return false;
    }

    //A state is containedExactly in the list when its key is present and value is the same, or its value is 0 and it is not present at all
    public bool ContainsExactly(GOAP_Worldstate state)
    {
        for (int i = 0; i < Count; i++)
        {
            if (this[i].key == state.key)
            {
                if (this[i].value == state.value)
                {
                    return true;
                }
            }                
        }

        return false;
    }

    public override string ToString()
    {
        string msg = "";
        for(int i = 0; i < Count; i++)
        {
            msg += this[i].ToString() + ",";
        }

        return msg;
    }
}
