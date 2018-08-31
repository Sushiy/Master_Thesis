using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class contains all info on individual actions. Most of all it holds the required and satisfyWorldstate fields, which are needed for planning.

public abstract class GOAP_Action
{
    private HashSet<KeyValuePair<string, object>> requiredWorldstates;
    private HashSet<KeyValuePair<string, object>> satisfyWorldstates;

    public float cost = 1f;

    public GameObject target;

    public virtual void Awake()
    {
        requiredWorldstates = new HashSet<KeyValuePair<string, object>>();
        satisfyWorldstates = new HashSet<KeyValuePair<string, object>>();
    }

    //Run this Action
    public virtual void Run(GOAP_Agent agent)
    {

    }

    //Check conditions that might change or need additional computation (like reachability)
    public abstract bool CheckProceduralConditions();

    public abstract bool RequiresInRange();

    
    private void AddRequiredWorldState(string state, object value)
    {
        requiredWorldstates.Add(new KeyValuePair<string, object>(state, value));
    }

    private void AddSatisfyWorldState(string state, object value)
    {
        satisfyWorldstates.Add(new KeyValuePair<string, object>(state, value));
    }

    public HashSet<KeyValuePair<string, object>> RequiredWorldstates
    {
        get
        {
            return requiredWorldstates;
        }
    }

    public HashSet<KeyValuePair<string, object>> SatisfyWorldStates
    {
        get
        {
            return satisfyWorldstates;
        }
    }

}
