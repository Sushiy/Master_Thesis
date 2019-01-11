using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class contains all info on individual actions. Most of all it holds the required and satisfyWorldstate fields, which are needed for planning.
public abstract class GOAP_Action :System.IEquatable<GOAP_Action>
{
    private HashSet<GOAP_Worldstate> requiredWorldstates;
    private HashSet<GOAP_Worldstate> satisfyWorldstates;

    protected float workCost = 1f;
    protected float coinCost = 0f;
    protected float range = 1.0f;

    private float secondsToWorkCostRation = 0.1f; // How much Time should pass for 1 workCost: 0.1f/1f
    private float workTime
    {
        get { return secondsToWorkCostRation * workCost; }
    }
    private float alphaWorkTime = 0.0f;
    protected bool isStartingWork
    {
        get { return alphaWorkTime == 0f; }
    }
    protected bool completed = false;

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
    protected IActionTarget target;
    public IActionTarget ActionTarget
    {
        get
        {
            return target;
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


    protected void Init()
    {
        requiredWorldstates = new HashSet<GOAP_Worldstate>();
        satisfyWorldstates = new HashSet<GOAP_Worldstate>();
    }
  

    //Perform this Action
    public abstract bool Perform(GOAP_Agent agent, float deltaTime);

    protected void StartPerform(GOAP_Agent agent)
    {
        //Only do this, when once at the beginning of the action
        if (alphaWorkTime != 0f) return;

        Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is performing: " + actionID + " Cost is: " + ActionCost + ((requiredSkill == null ) ? "" : " reduced by:" + requiredSkill.id.ToString()) );
        agent.View.PrintMessage(ActionID);
    }

    protected void CompletePerform(GOAP_Agent agent)
    {
        foreach (GOAP_Worldstate state in satisfyWorldstates)
        {
            agent.ChangeCurrentWorldState(state);
        }
    }

    protected void UpdateWorkTime(float deltaTime)
    {
        alphaWorkTime += deltaTime;
        if (alphaWorkTime >= workTime)
        {
            completed = true;
        }
    }

    //Check conditions that might change or need additional computation (like reachability)
    public abstract bool CheckProceduralConditions(GOAP_Agent agent);

    //Returns true if the requirements are still satisfied, false if they are not
    public bool CheckRequirements(GOAP_Agent agent)
    {
        if(CheckProceduralConditions(agent))
        {
            foreach (GOAP_Worldstate state in requiredWorldstates)
            {
                if (!agent.currentWorldstates.Contains(state))
                {
                    return false;
                }
            }
            return true;
        }
        return false;

    }

    public void ApplySkillModifier(float skillModifier)
    {
        this.workCost *= skillModifier;
    }

    /*
    public void SetCoinCost(float coinCost)
    {
        this.coinCost = coinCost;
    }
    */

    public abstract bool RequiresInRange();

    public bool IsInRange(GOAP_Agent agent)
    {
        if (!RequiresInRange()) return true;
        if (RequiresInRange() && target != null && agent.View.IsInRange(target.GetPosition(), range))
        {
            return true; //TODO: put some actual rangeTesting in here, dependant on the target
        }
        return false;
    }



    protected void AddRequiredWorldState(WorldStateKey key, bool value, IActionTarget target = null)
    {
        AddRequiredWorldState(key, value ? 1 : 0, target);
    }
    protected void AddRequiredWorldState(WorldStateKey key, int value, IActionTarget target = null)
    {
        GOAP_Worldstate state;
        state.key = key;
        state.target = target;
        state.value = value;
        requiredWorldstates.Add(state);
    }
    protected void AddSatisfyWorldState(WorldStateKey key, bool value, IActionTarget target = null)
    {
        AddSatisfyWorldState(key, value ? 1 : 0, target);
    }
    protected void AddSatisfyWorldState(WorldStateKey key, int value, IActionTarget target = null)
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

    public HashSet<GOAP_Worldstate> SatisfyWorldstates
    {
        get
        {
            return satisfyWorldstates;
        }
    }
}
