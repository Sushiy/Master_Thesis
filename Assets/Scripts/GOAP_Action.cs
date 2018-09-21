using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class contains all info on individual actions. Most of all it holds the required and satisfyWorldstate fields, which are needed for planning.

public abstract class GOAP_Action
{
    private HashSet<GOAP_Worldstate> requiredWorldstates;
    private HashSet<GOAP_Worldstate> satisfyWorldstates;

    public float cost = 1f;

    public GameObject target;

    public virtual void Awake()
    {
        requiredWorldstates = new HashSet<GOAP_Worldstate>();
        satisfyWorldstates = new HashSet<GOAP_Worldstate>();
    }

    //Run this Action
    public virtual void Run(GOAP_Agent agent)
    {

    }

    //Check conditions that might change or need additional computation (like reachability)
    public abstract bool CheckProceduralConditions();

    public abstract bool RequiresInRange();

    
    private void AddRequiredWorldState(WorldStateKey key, GameObject target, object value)
    {
        GOAP_Worldstate state;
        state.key = key;
        state.target = target;
        state.value = value;
        requiredWorldstates.Add(state);
    }

    private void AddSatisfyWorldState(WorldStateKey key, GameObject target, object value)
    {
        GOAP_Worldstate state;
        state.key = key;
        state.target = target;
        state.value = value;
        satisfyWorldstates.Add(state);
    }

    public HashSet<GOAP_Worldstate> RequiredWorldstates
    {
        get
        {
            return requiredWorldstates;
        }
    }

    public HashSet<GOAP_Worldstate> SatisfyWorldStates
    {
        get
        {
            return satisfyWorldstates;
        }
    }

}
