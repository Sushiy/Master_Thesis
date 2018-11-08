using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class contains all info on individual actions. Most of all it holds the required and satisfyWorldstate fields, which are needed for planning.

public abstract class GOAP_Action :MonoBehaviour, System.IEquatable<GOAP_Action>
{
    private HashSet<GOAP_Worldstate> requiredWorldstates;
    private HashSet<GOAP_Worldstate> satisfyWorldstates;


    [HideInInspector]
    protected float workCost = 1f;
    protected float coinCost = 0f;
    public float ActionCost
    {
        get
        {
            return workCost;
        }
    }
    public float CoinCost
    {
        get
        {
            return coinCost;
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
    protected bool keepOpen = false;
    public bool KeepOpen
    {
        get
        {
            return keepOpen;
        }
    }

    protected GOAP_Skill requiredSkill = null;
    public GOAP_Skill RequiredSkill
    {
        get
        {
            return requiredSkill;
        }
    }


    public virtual void Awake()
    {
        requiredWorldstates = new HashSet<GOAP_Worldstate>();
        satisfyWorldstates = new HashSet<GOAP_Worldstate>();
    }
  

    //Run this Action
    public abstract bool Run(GOAP_Agent agent);

    //Check conditions that might change or need additional computation (like reachability)
    public abstract bool CheckProceduralConditions(GOAP_Agent agent);

    //public abstract void UpdateCosts(GOAP_Agent agent);

    public abstract bool RequiresInRange();

    public bool IsInRange(GOAP_Agent agent)
    {
        if (!RequiresInRange()) return true;
        if (RequiresInRange() && target != null && Vector3.Distance(target.transform.position, agent.transform.position) < 1.0f)
        {
            return true; //TODO: put some actual rangeTesting in here, dependant on the target
        }
        return false;
    }


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

    bool IEquatable<GOAP_Action>.Equals(GOAP_Action other)
    {
        if (other == null) return false;
        return other.actionID.Equals(actionID);
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
