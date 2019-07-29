using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This class contains all info on individual actions. Most of all it holds the required and satisfyWorldstate fields, which are needed for planning.
public abstract class GOAP_Action :System.IEquatable<GOAP_Action>
{
    public class VariationData
    {
        public List_GOAP_Worldstate RequiredWorldstates { private set; get; }
        public List_GOAP_Worldstate SatisfyWorldstates { private set; get; }
        public float workCost;
        public float range;
        public Skills benefitingSkill;

        public VariationData()
        {
            RequiredWorldstates = new List_GOAP_Worldstate();
            SatisfyWorldstates = new List_GOAP_Worldstate();
            workCost = 1f;
            range = 1f;
            benefitingSkill = Skills.None;
        }

        public VariationData(List_GOAP_Worldstate required, List_GOAP_Worldstate satisfy, float workCost = 1, float range = 1, Skills benefitingSkill = Skills.None)
        {
            RequiredWorldstates = required;
            SatisfyWorldstates = satisfy;
            this.workCost = workCost;
            this.range = range;
            this.benefitingSkill = benefitingSkill;
        }

        public void AddRequiredWorldState(WorldStateKey key, bool value, IActionTarget target = null)
        {
            AddRequiredWorldState(key, value ? 1 : 0, target);
        }
        public void AddRequiredWorldState(WorldStateKey key, int value, IActionTarget target = null)
        {
            GOAP_Worldstate state = new GOAP_Worldstate(key, value, target);
            RequiredWorldstates.Add(state);
        }
        public void AddSatisfyWorldState(WorldStateKey key, bool value, IActionTarget target = null)
        {
            AddSatisfyWorldState(key, value ? 1 : 0, target);
        }
        public void AddSatisfyWorldState(WorldStateKey key, int value, IActionTarget target = null)
        {
            GOAP_Worldstate state = new GOAP_Worldstate(key, value, target);
            SatisfyWorldstates.Add(state);
        }
    }

    public List<VariationData> variations;
    protected int variationIndex;

    public static string[] baseActions =
    {
        "Action_EatFood",
        "Action_Sleep",
        "Action_CheckForQuest",
        "Action_PostQuest",
        "Action_WaitForQuest",
        "Action_CompleteQuest"
    };

    public List_GOAP_Worldstate RequiredWorldstates { private set; get; }
    public List_GOAP_Worldstate SatisfyWorldstates { private set; get; }

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

    public Skills BenefitingSkill
    {
        protected set;
        get;
    }


    protected void Init()
    {
        RequiredWorldstates = new List_GOAP_Worldstate();
        SatisfyWorldstates = new List_GOAP_Worldstate();
        variations = new List<VariationData>();
    }

    //Perform this Action
    public abstract bool Perform(GOAP_Agent agent, float deltaTime);

    protected void StartPerform(GOAP_Agent agent)
    {
        //Only do this, when once at the beginning of the action
        if (alphaWorkTime != 0f) return;

        agent.Character.Log("<color=#0000cc><b>PERFORMING</b>: " + agent.Character.characterData.characterName + "</color>:" + actionID + "(" + ActionCost + ((BenefitingSkill == Skills.None ) ? ")" : (" reduced by:" + BenefitingSkill.ToString()) + ")") );
        agent.View.PrintMessage(ActionID);
    }

    protected void CompletePerform(GOAP_Agent agent)
    {
        foreach (GOAP_Worldstate state in SatisfyWorldstates)
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
            foreach (GOAP_Worldstate state in RequiredWorldstates)
            {
                if (!agent.currentWorldstates.ContainsExactly(state))
                {
                    if (state.IsObservableState && !agent.currentWorldstates.ContainsKey(state))
                    {
                        agent.ChangeCurrentWorldState(state);
                        agent.Character.Log("<color=#cc00cc>" + agent.Character.characterData.characterName + "</color> assumes state:" + state.ToString());
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

    public bool HasVariations()
    {
        return variations != null && variations.Count > 0;
    }

    protected void SetVariationData(int i)
    {
        RequiredWorldstates = variations[i].RequiredWorldstates;
        SatisfyWorldstates = variations[i].SatisfyWorldstates;
        workCost = variations[i].workCost;
        range = variations[i].range;
        BenefitingSkill = variations[i].benefitingSkill;
        variationIndex = i;
    }

    public abstract GOAP_Action GetVariation(int i);

    protected void AddRequiredWorldState(WorldStateKey key, bool value, IActionTarget target = null)
    {
        AddRequiredWorldState(key, value ? 1 : 0, target);
    }
    protected void AddRequiredWorldState(WorldStateKey key, int value, IActionTarget target = null)
    {
        GOAP_Worldstate state = new GOAP_Worldstate(key, value, target);
        RequiredWorldstates.Add(state);
    }
    protected void AddSatisfyWorldState(WorldStateKey key, bool value, IActionTarget target = null)
    {
        AddSatisfyWorldState(key, value ? 1 : 0, target);
    }
    protected void AddSatisfyWorldState(WorldStateKey key, int value, IActionTarget target = null)
    {
        GOAP_Worldstate state = new GOAP_Worldstate(key, value, target);
        SatisfyWorldstates.Add(state);
    }

    bool IEquatable<GOAP_Action>.Equals(GOAP_Action other)
    {
        if (other == null) return false;
        return other.actionID.Equals(actionID);
    }

    public static bool IsQuestActionID(string id)
    {
        return id == "Action_PostQuest" || id == "Action_WaitForQuest" || id == "Action_CompleteQuest";
    }

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
}
