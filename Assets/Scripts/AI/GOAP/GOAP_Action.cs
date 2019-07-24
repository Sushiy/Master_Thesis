using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This class contains all info on individual actions. Most of all it holds the required and satisfyWorldstate fields, which are needed for planning.
public abstract class GOAP_Action :System.IEquatable<GOAP_Action>
{
    public static string[] baseActions =
    {
        "Action_EatFood",
        "Action_Sleep",
        "Action_CheckForQuest",
        "Action_PostQuest",
        "Action_WaitForQuest",
        "Action_CompleteQuest",
        "Action_GetWater",
        "Action_GatherDeadwood"
    };

    private static string[] allActions;
    public static string[] GetAllActionNames()
    {
        System.Type[] types = typeof(GOAP_Action).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GOAP_Action))).ToArray();

        if (allActions == null || allActions.Length != types.Length)
        {
            allActions = new string[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                string actionName = types[i].ToString();
                allActions[i] = actionName;
            }
        }
        return allActions;
    }

    private List_GOAP_Worldstate requiredWorldstates;
    private List_GOAP_Worldstate satisfyWorldstates;

    protected float workCost = 1f;
    protected float range = 1.0f;

    private float secondsToWorkCostRatio = 0.25f; // How much Time should pass for 1 workCost: 0.1s per 1 cost
    private float workTime
    {
        get { return secondsToWorkCostRatio * workCost; }
    }
    protected float alphaWorkTime = 0.0f;
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
        requiredWorldstates = new List_GOAP_Worldstate();
        satisfyWorldstates = new List_GOAP_Worldstate();
    }

    //Perform this Action
    public abstract bool Perform(GOAP_Agent agent, float deltaTime);

    protected void StartPerform(GOAP_Agent agent)
    {
        //Only do this, when once at the beginning of the action
        if (alphaWorkTime != 0f) return;

        Debug.Log("<color=#0000cc><b>PERFORMING</b>: " + agent.Character.characterData.characterName + "</color>:" + actionID + "(" + ActionCost + ((requiredSkill == null ) ? ")" : (" reduced by:" + requiredSkill.id.ToString()) + ")") );
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

    //Returns true if the requirements are satisfied, false if they are not
    public bool CheckRequirements(GOAP_Agent agent)
    {
        if(CheckProceduralConditions(agent))
        {
            foreach (GOAP_Worldstate state in requiredWorldstates)
            {
                if (!agent.currentWorldstates.ContainsExactly(state))
                {
                    if (state.IsObservableState && !agent.currentWorldstates.ContainsKey(state))
                    {
                        agent.ChangeCurrentWorldState(state);
                        Debug.Log("<color=#cc00cc>" + agent.Character.characterData.characterName + "</color> assumes state:" + state.ToString());
                    }
                    else
                    {
                        return false;
                    }
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
        GOAP_Worldstate state = new GOAP_Worldstate(key, value, target);
        requiredWorldstates.Add(state);
    }
    protected void AddSatisfyWorldState(WorldStateKey key, bool value, IActionTarget target = null)
    {
        AddSatisfyWorldState(key, value ? 1 : 0, target);
    }
    protected void AddSatisfyWorldState(WorldStateKey key, int value, IActionTarget target = null)
    {
        GOAP_Worldstate state = new GOAP_Worldstate(key, value, target);
        satisfyWorldstates.Add(state);
    }

    bool IEquatable<GOAP_Action>.Equals(GOAP_Action other)
    {
        if (other == null) return false;
        return other.actionID.Equals(actionID);
    }

    public List_GOAP_Worldstate RequiredWorldstates
    {
        get
        {
            return requiredWorldstates;
        }
    }

    public List_GOAP_Worldstate SatisfyWorldstates
    {
        get
        {
            return satisfyWorldstates;
        }
    }
}
