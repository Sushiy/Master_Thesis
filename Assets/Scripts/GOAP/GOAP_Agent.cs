using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum FSM_State
{
    IDLE,
    PERFORMACTION,
    MOVETO
}

public class GOAP_Agent
{
    FSM_State currentState = FSM_State.IDLE;

    HashSet<GOAP_Worldstate> standardGoal;
    public HashSet<GOAP_Worldstate> goal;

    public HashSet<GOAP_Worldstate> currentWorldstates;

    public PlannableActions actions;

    Queue<GOAP_Action> currentActions;
    GOAP_Action activeAction;

    private GOAP_Character character;
    public GOAP_Character Character
    {
        get { return character; }
    }

    public float planningWaitTimer = 3.0f;
    private float timeSincePlanned = 0.0f;
    bool allowedToPlan = true;
    bool actionCompleted = true;

    public GOAP_Quest postedQuest = null;
    public GOAP_Quest activeQuest = null;

    private IGOAP_AgentView view;
    public IGOAP_AgentView View
    {
        get { return view; }
    }

    public GOAP_Agent(GOAP_Character character, IGOAP_AgentView view)
    {
        this.character = character;
        this.view = view;

        currentActions = new Queue<GOAP_Action>();
        currentWorldstates = new HashSet<GOAP_Worldstate>();

        standardGoal = new HashSet<GOAP_Worldstate>(); // TODO: implement proper goals for agents; => agent.getCurrentGoal();
        standardGoal.Add(new GOAP_Worldstate(WorldStateKey.bHasWood, 1, null)); //TODO: remove this when I have proper goals
        goal = standardGoal;
    }

    // Update is called once per frame
    public void Update(float deltaTime)
    {
        if (currentState == FSM_State.IDLE && postedQuest == null)
        {
            if (allowedToPlan)
            {

                activeQuest = CheckForQuests();
                if (activeQuest == null) goal = standardGoal;
                else goal = activeQuest.RequiredStates;
                Queue<GOAP_Action> newPlan;
                //Fetch a new Plan from the planner
                if (character.availableActions != PlannableActions.None)
                {
                    newPlan = GOAP_Planner.instance.Plan(this, goal, FetchWorldState(), character.availableActions);
                }
                else
                {
                    newPlan = GOAP_Planner.instance.Plan(this, goal, FetchWorldState());
                }
                if (newPlan != null)
                {
                    //do what the plan says!
                    currentActions = newPlan;
                    currentState = FSM_State.PERFORMACTION;
                }
                else
                {
                    //try again? or something...
                    Debug.Log("No plan?");
                }
                allowedToPlan = false;
                timeSincePlanned = 0.0f;
            }
            else
            {
                timeSincePlanned += deltaTime;
                if (timeSincePlanned > planningWaitTimer)
                    allowedToPlan = true;
            }
        }

        else if (currentState == FSM_State.PERFORMACTION)
        {
            //Use one of the currentActions
            if (actionCompleted)
            {
                if (currentActions.Count > 0)
                {
                    activeAction = currentActions.Dequeue();

                }
                else
                {
                    if (activeQuest != null)
                    {
                        Debug.Log("<color=#0000cc>" + character.characterName + "</color> completed Quest " + activeQuest.id);

                        activeQuest.Complete();
                        activeQuest = null;
                    }
                    activeAction = null;
                }

            }

            if (activeAction != null)
            {
                if (!activeAction.IsInRange(this))
                {
                    currentState = FSM_State.MOVETO;
                }
                else
                {
                    actionCompleted = activeAction.Run(this);
                }
            }
            else
            {
                currentState = FSM_State.IDLE;
            }
        }

        else if (currentState == FSM_State.MOVETO)
        {
            //Move to the target!
            if (!activeAction.IsInRange(this))
            {
                view.MoveTo(activeAction.ActionTarget.GetPosition());
            }
            else
            {
                currentState = FSM_State.PERFORMACTION;
            }
        }
    }

    public GOAP_Quest CheckForQuests()
    {
        GOAP_Quest result = null;
        foreach (GOAP_Quest quest in GOAP_QuestBoard.instance.quests)
        {
            if (quest.Owner != this)
            {
                result = quest;
                break;
            }
        }
        if (result != null)
        {
            Debug.Log("<color=#0000cc>" + character.characterName + "</color> chose Quest " + result.id);
            GOAP_QuestBoard.instance.ChooseQuest(result);
        }
        return result;
    }

    private IEnumerator DelayPlanning()
    {
        allowedToPlan = false;
        yield return new WaitForSeconds(planningWaitTimer);
        allowedToPlan = true;
    }

    private HashSet<GOAP_Worldstate> FetchWorldState()
    {
        return currentWorldstates;
    }

    public void ChangeCurrentWorldState(GOAP_Worldstate newState)
    {
        //If the newState is a uniqueState and its key is already in the currentstate, we will update its value, by removing and readding (the value is not part of the hashcode for unique states)
        if (newState.IsUniqueState() && currentWorldstates.Contains(newState))
        {
            currentWorldstates.Remove(newState);
            currentWorldstates.Add(newState);
        }
        //Otherwise, if it is not a uniquestate and also the newstate is not contained in the currentworldstate, add it
        else if (!newState.IsUniqueState() && !currentWorldstates.Contains(newState))
        {
            currentWorldstates.Add(newState);
        }
    }

    public void RemoveCurrentWorldState(GOAP_Worldstate newState)
    {
        if (currentWorldstates.Contains(newState))
        {
            currentWorldstates.Remove(newState);
        }
    }
}