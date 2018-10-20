using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class contains all info on individual actions. Most of all it holds the required and satisfyWorldstate fields, which are needed for planning.

public abstract class GOAP_Action : MonoBehaviour
{
    private HashSet<GOAP_Worldstate> requiredWorldstates;
    private HashSet<GOAP_Worldstate> satisfyWorldstates;

    [HideInInspector]
    protected float cost = 1f;
    public float ActionCost
    {
        get
        {
            return cost;
        }
    }
    [HideInInspector]
    protected string actionID = "";
    public string ActionID
    {
        get
        {
            return actionID;
        }
    }
    protected GameObject target;
    public GameObject ActionTarget
    {
        get
        {
            return target;
        }
    }

    public virtual void Awake()
    {
        requiredWorldstates = new HashSet<GOAP_Worldstate>();
        satisfyWorldstates = new HashSet<GOAP_Worldstate>();
    }

    //Run this Action
    public abstract void Run(GOAP_Agent agent);

    //Check conditions that might change or need additional computation (like reachability)
    public abstract bool CheckProceduralConditions();

    public abstract bool RequiresInRange();

    
    protected void AddRequiredWorldState(WorldStateKey key, bool value, GameObject target = null)
    {
        GOAP_Worldstate state;
        state.key = key;
        state.target = target;
        state.value = value;
        requiredWorldstates.Add(state);
    }

    protected void AddSatisfyWorldState(WorldStateKey key, bool value, GameObject target = null)
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
