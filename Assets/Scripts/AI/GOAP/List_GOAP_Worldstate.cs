using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_GOAP_Worldstate : List<GOAP_Worldstate>
{
    public List_GOAP_Worldstate():base()
    {

    }

    public List_GOAP_Worldstate(List_GOAP_Worldstate list) : base(list)
    {

    }

    public List_GOAP_Worldstate(List<GOAP_Worldstate> list):base(list)
    {

    }

    public List_GOAP_Worldstate(GOAP_Worldstate[] array):base(array)
    {

    }

    public bool ContainsKey(GOAP_Worldstate state)
    {
        for(int i = 0; i < Count; i++)
        {
            if(this[i].key == state.key && (state.IsUniqueState() || this[i].value == state.value))
            {
                return true;
            }
        }

        return false;
    }

    //A state is containedExactly in the list when its key is present and value is the same, or its value is 0 and it is not present at all
    public bool ContainsExactly(GOAP_Worldstate state)
    {
        bool hasKey = false;
        for (int i = 0; i < Count; i++)
        {
            if (this[i].key == state.key)
            {
                hasKey = true;
                if (this[i].value == state.value)
                {
                    return true;
                }
            }                
        }

        if(!hasKey && state.IsUniqueState() && state.value == 0)
        {
            return true;
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
