using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldStateKey
{
    bHasAxe,
    bHasLog,
    bHasWood,
    iStoredWood
}

public struct GOAP_Worldstate
{
    //from the above list
    public WorldStateKey key;

    //Can be null
    public GameObject target;
    //This is unsafe, each WorldstateKey signifies which type of value it should receive, but this can't be properly checked.
    public object value;
    
}

