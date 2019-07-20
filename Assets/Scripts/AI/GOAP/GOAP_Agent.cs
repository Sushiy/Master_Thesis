using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum FSM_State
{
    IDLE,
    PLANNING,
    PERFORMACTION,
    MOVETO,
    WAITFORCALL,
}

public class GOAP_Agent
{
    FSM_State currentState = FSM_State.IDLE;
    FSM_State stateBeforeWait = FSM_State.IDLE;

    public List_GOAP_Worldstate activeGoal;

    public List_GOAP_Worldstate checkedCharacterGoals;

    public List<int> checkedQuestIds;

    public List_GOAP_Worldstate currentWorldstates;

    public PlannableActions actions;

    Queue<GOAP_Action> currentActions;
    public GOAP_Action activeAction;

    private GOAP_Character character;
    public GOAP_Character Character
    {
        get { return character; }
    }

    float planningWaitTimer = 5.0f;
    public float timeSincePlanned = 0.0f;
    public bool AllowedToPlan
    {
        get { return timeSincePlanned >= planningWaitTimer; }
    }
    bool actionCompleted = true;

    float waitForCallTimer = 4.0f; //seconds to wait before moving again after being called
    float timeWaitingForCall = 0.0f;

    float callInterval = 4.0f; //seconds between calls
    float timeSinceCall = 0.0f;

    public List<int> postedQuestIDs = null;
    public List<int> completedQuestIDs = null;
    
    public Dictionary<int, Queue<GOAP_Action>> questPlans;

    public GOAP_Quest activeQuest = null;
    
    public IGOAP_AgentView View
    {
        private set;
        get;
    }

    public List<PlanInfo> planMemory;
    public int? activePlanInfo;

    public GOAP_Agent(GOAP_Character character, IGOAP_AgentView view)
    {
        this.character = character;
        this.View = view;

        checkedCharacterGoals = new List_GOAP_Worldstate();

        currentActions = new Queue<GOAP_Action>();
        currentWorldstates = new List_GOAP_Worldstate();
        checkedQuestIds = new List<int>();

        postedQuestIDs = new List<int>();
        completedQuestIDs = new List<int>();

        questPlans = new Dictionary<int, Queue<GOAP_Action>>();
        activeGoal = new List_GOAP_Worldstate();

        planMemory = new List<PlanInfo>();
    }
    public void ChooseGoal()
    {
        activeGoal = new List_GOAP_Worldstate();
        string msg = "<color=#0000cc><b>CHECKING GOALS</b>:" + character.characterName + "</color>\n";

        //Check goals top to bottom, to see which need to be fulfilled
        for (int i = 0; i < character.goals.Count; i++)
        {
            GOAP_Worldstate goal = character.goals[i];

            //Check if the goal is already satisfied
            if(!GOAP_Planner.instance.IsGoalSatisfied(currentWorldstates, goal))
            {
                //And if it has been checked before
                if(!checkedCharacterGoals.ContainsExactly(goal))
                {
                    checkedCharacterGoals.Add(character.goals[i]);
                    activeGoal.Add(goal);
                    msg += character.goals[i].key + ":" + character.goals[i].value + " not yet satisfied\n";
                    break;
                }
                else
                {
                    msg += character.goals[i].key + ":" + character.goals[i].value + " not yet satisfied, but was already checked\n";
                }
            }
            else
            {
                if(checkedCharacterGoals.ContainsExactly(character.goals[i]))
                {
                    checkedCharacterGoals.Remove(character.goals[i]);
                }
                msg += character.goals[i].key + ":" + character.goals[i].value + " already satisfied\n";
            }
        }
        Debug.Log(msg);

        
        //if none of the goals needed to be fulfilled, instead check the quests
        if(activeGoal.Count == 0)
        {
            activeQuest = CheckForQuests();
            if(activeQuest != null)
            {
                Debug.Log("<color=#0000cc>" + character.characterName + "</color> has fulfilled all of his goals. But found a quest");
                activeGoal.AddRange(activeQuest.RequiredStates);
            }
            else
            {
                Debug.Log("<color=#0000cc>" + character.characterName + "</color> has fulfilled all of his goals. No quest");
                timeSincePlanned = 0.0f;
            }
        }
    }

    // Update is called once per frame
    public void Update(float deltaTime)
    {
        switch(currentState)
        {
            case FSM_State.IDLE:
                IdleUpdate(deltaTime);
                break;
            case FSM_State.PLANNING:
                PlanningUpdate(deltaTime);
                break;
            case FSM_State.MOVETO:
                MoveToUpdate(deltaTime);
                break;
            case FSM_State.PERFORMACTION:
                PerformUpdate(deltaTime);
                break;
            case FSM_State.WAITFORCALL:
                WaitForCallUpdate(deltaTime);
                break;
        }
    }

    private void ChangeState(FSM_State newState)
    {
        //EndStateActions
        switch (currentState)
        {
            case FSM_State.IDLE:
                break;
            case FSM_State.PLANNING:
                timeSincePlanned = 0.0f;
                break;
            case FSM_State.MOVETO:
                break;
            case FSM_State.PERFORMACTION:
                break;
            case FSM_State.WAITFORCALL:
                break;
        }

        //StartStateActions
        switch (newState)
        {
            case FSM_State.IDLE:
                break;
            case FSM_State.PLANNING:
                break;
            case FSM_State.MOVETO:
                break;
            case FSM_State.PERFORMACTION:
                break;
            case FSM_State.WAITFORCALL:
                if (currentState != FSM_State.WAITFORCALL)
                {
                    stateBeforeWait = currentState;
                }
                timeWaitingForCall = 0.0f;
                break;
        }

        currentState = newState;
    }


    private void IdleUpdate(float deltaTime)
    {
        if (AllowedToPlan || completedQuestIDs.Count > 0)
        {
            ChangeState(FSM_State.PLANNING);
        }
        else
        {
            View.PrintMessage("Idle");
        }
        timeSincePlanned += deltaTime;
    }

    private void PlanningUpdate(float deltaTime)
    {
        View.PrintMessage("Planning");
        #region cleanup Useless Quests

        //First check if any of the posted quests are already done so they can be removed
        List<int> alreadySolvedQuests = new List<int>();
        for (int i = 0; i < postedQuestIDs.Count; i++)
        {
            if(IsSatisfiedInCurrentWorldstate(GOAP_QuestBoard.instance.quests[postedQuestIDs[i]].RequiredStates))
            {
                Debug.Log("<color=#0000cc>" + character.characterName + "</color>s Quest " + postedQuestIDs[i] + " was already solved.");
                alreadySolvedQuests.Add(postedQuestIDs[i]);
            }
        }

        for (int i = 0; i < alreadySolvedQuests.Count; i++)
        {
            GOAP_QuestBoard.instance.CompleteQuest(alreadySolvedQuests[i]);
        }

        #endregion

        //Actual planning:

        if (activeGoal.Count > 0)
        {
            Queue<GOAP_Action> newPlan;
            //Fetch a new Plan from the planner
            planMemory.Add(new PlanInfo(PrintGoal(), Character.characterName, activeQuest != null ? activeQuest.id : -1));
            newPlan = GOAP_Planner.instance.Plan(this, activeGoal, currentWorldstates, character.availableActions);

            timeSincePlanned = 0.0f;

            if (newPlan != null)
            {
                //do what the plan says!
                currentActions = newPlan;
                actionCompleted = true;
                planMemory[planMemory.Count - 1].ApprovePlan(planMemory.Count - 1, PrintActionQueue());
                activePlanInfo = planMemory.Count - 1;
                ChangeState(FSM_State.PERFORMACTION);
            }
            else
            {
                planMemory.RemoveAt(planMemory.Count - 1);
                //try again? or something...
                Debug.Log("No plan?");
                if (activeQuest != null)
                {
                    activeQuest = null;
                    activeGoal.Clear();
                    ChangeState(FSM_State.IDLE);
                }
            }
        }
        else
        {
            //Before checking goals, check if any of your quests have been completed
            if (completedQuestIDs.Count > 0)
            {
                //choose the first one and go
                int id = completedQuestIDs[0];
                if (questPlans.ContainsKey(id))
                {
                    currentActions = new Queue<GOAP_Action>(questPlans[id]);
                    questPlans.Remove(id);
                    ChangeState(FSM_State.PERFORMACTION);
                    Debug.Log("<color=#0000cc>" + character.characterName + "</color> has completed Quest " + id + " to finish. It includes " + currentActions.Count + " actions and starts with " + currentActions.Peek());
                }
                else
                {
                    string msg = "";
                    foreach (int key in questPlans.Keys)
                    {
                        msg += key + ",";
                    }
                    Debug.LogError("<color=#0000cc>" + character.characterName + "</color> tried to continue Quest " + id + " but didnt find a corresponding plan.\nExisting plans:" + msg);
                    ChangeState(FSM_State.IDLE);
                }
            }
            else
            {
                if (postedQuestIDs.Count == 0)
                {
                    checkedCharacterGoals.Clear();
                }
                ChooseGoal();
                if(activeGoal.Count == 0)
                {
                    ChangeState(FSM_State.IDLE);
                }
            }

        }
    }

    private void PerformUpdate(float deltaTime)
    {
        //Get the next currentAction
        if (actionCompleted)
        {
            if (currentActions.Count > 1 || (currentActions.Count > 0 && activeAction == null))
            {
                //Remove the old active action
                if (activeAction != null)
                    currentActions.Dequeue();
                //Get the new active action
                activeAction = currentActions.Peek();
                actionCompleted = false;
            }
            else
            {
                activeAction = null;
            }

        }

        if (activeAction != null)
        {
            if (activeAction.CheckRequirements(this))
            {
                if(activeAction.SatisfyWorldstates.Count > 0 && IsSatisfiedInCurrentWorldstate(activeAction.SatisfyWorldstates))
                {
                    actionCompleted = true;
                    Debug.Log("<color=#0000cc>" + character.characterName + "</color> didn't need to perform <color=#cc0000>" + activeAction.ActionID + "</color> anymore.");
                }
                else
                {
                    if (!activeAction.IsInRange(this))
                    {
                        ChangeState(FSM_State.MOVETO);
                        View.PrintMessage("MoveTo " + activeAction.ActionID);
                    }
                    else
                    {
                        actionCompleted = activeAction.Perform(this, deltaTime);
                        View.VisualizeAction(activeAction);
                    }

                }
                    
            }
            else
            {
                Debug.Log("<color=#0000cc>" + character.characterName + "</color> cannot perform <color=#cc0000>" + activeAction.ActionID + "</color> anymore.");
                CancelPlan();
            }

        }
        else
        {
            Debug.Log("<color=#0000cc>" + character.characterName + "</color> doesn't have any actions left anymore.");
            activeGoal.Clear();
            ChangeState(FSM_State.IDLE);
        }
    }

    private void MoveToUpdate(float deltaTime)
    {
        //Move to the target!
        if (!activeAction.IsInRange(this))
        {
            if(timeSinceCall >= callInterval)
            {
                activeAction.ActionTarget.Call(View.GetPosition());
            }
            else
            {
                timeSinceCall += deltaTime;
            }
            View.MoveTo(activeAction.ActionTarget.GetPosition());
        }
        else
        {
            ChangeState(FSM_State.PERFORMACTION);
        }
    }

    private void WaitForCallUpdate(float deltaTime)
    {
        if(timeWaitingForCall >= waitForCallTimer)
        {
            currentState = stateBeforeWait;
        }
        View.PrintMessage("Called");
        timeWaitingForCall += deltaTime;
    }


    public void CancelPlan()
    {
        if(activeQuest != null)
        {
            checkedQuestIds.Remove(activeQuest.id);
        }
        activePlanInfo = null;
        activeAction = null;
        currentActions.Clear();
        actionCompleted = true;
        ChangeState(FSM_State.IDLE);
    }

    public GOAP_Quest CheckForQuests()
    {
        if(GOAP_QuestBoard.instance.quests.Count == 0)
        {
            checkedQuestIds.Clear();
            return null;
        }
        GOAP_Quest result = null;
        //First go through all questIds we have tried to plan for before and see if they are even still available
        //I use a quick check here, that only deletes ids that are lower than the lowest quest on the board
        //This is basically just garbagecollection
        int minKey = GOAP_QuestBoard.instance.quests.First().Key;
        for (int i = 0; i < checkedQuestIds.Count; i++)
        {
            //The lowest quest has the lowest id
            if(minKey > checkedQuestIds[i])
            {
                checkedQuestIds.Remove(checkedQuestIds[i]);
            }
        }

        foreach (KeyValuePair<int, GOAP_Quest> quest in GOAP_QuestBoard.instance.quests)
        {
            //if we don't own this quest, lets try to plan for it
            if (quest.Value.Owner != this && !checkedQuestIds.Contains(quest.Value.id))
            {
                result = quest.Value;
                break;
            }
        }
        if (result != null)
        {
            Debug.Log("<color=#0000cc>" + character.characterName + "</color> chose to plan for Quest " + result.id);
            checkedQuestIds.Add(result.id);
        }
        return result;
    }

    public void ChangeCurrentWorldState(WorldStateKey key, bool value)
    {
        ChangeCurrentWorldState(key, value ? 1 : 0);
    }

    public void ChangeCurrentWorldState(WorldStateKey key, int value)
    {
        ChangeCurrentWorldState(new GOAP_Worldstate(key, value));
    }

    public void ChangeCurrentWorldState(GOAP_Worldstate newState)
    {
        if(currentWorldstates.ContainsKey(newState))
        {
            if(newState.IsUniqueState())
            {
                if (newState.value > 0)
                {
                    currentWorldstates.Remove(newState); //this should find the state based on the key only
                    currentWorldstates.Add(newState);
                    Debug.Log(Character.name + " <color=#cc0000>Updated state:</color> " + newState.ToString());
                }
                else
                {
                    RemoveCurrentWorldState(newState);
                }

            }
            else
            {
                if(!currentWorldstates.ContainsExactly(newState))
                {
                    Debug.Log(Character.name + " <color=#cc0000>Add state:</color> " + newState.ToString());
                    currentWorldstates.Add(newState);
                }

            }
        }

        //Otherwise, if the newstate is not contained in the currentworldstate, add it
        else
        {
            Debug.Log(Character.name + " <color=#cc0000>Add state:</color> " + newState.ToString());
            currentWorldstates.Add(newState);
        }
    }

    public void RemoveCurrentWorldState(ItemType type)
    {
        GOAP_Worldstate state = new GOAP_Worldstate(WorldStateKey.eHasItem, (int)type);
        RemoveCurrentWorldState(state);
    }

    public void RemoveCurrentWorldState(GOAP_Worldstate state)
    {
        if (currentWorldstates.ContainsKey(state))
        {
            Debug.Log(Character.name + " <color=#cc0000>Remove state:</color> " + state.ToString());
            currentWorldstates.Remove(state);
        }
    }

    public string PrintGoal()
    {
        string msg = "";
        if (activeGoal.Count < 1) return "None";
        foreach(GOAP_Worldstate state in activeGoal)
        {
            msg += state.ToString() + "\n";
        }
        if(activeQuest != null)
        {
            msg += "(Quest " + activeQuest.id + ")";
        }
        else
        {
            msg += "(Personal)";
        }
        return msg;
    }

    public string PrintCurrentWorldstates()
    {
        string msg = Character.name + " Current Worldstates\n";
        foreach (GOAP_Worldstate state in currentWorldstates)
        {
            msg += state.ToString() + "\n";
        }

        return msg;
    }

    public void ResetPlanningTimer()
    {
        timeSincePlanned = 0f;
    }

    public void UpdateActionQueue(Queue<GOAP_Action> actionQueue)
    {
        //TODO: I would like to enqueue the whole queue instead of replacing it
        currentActions = actionQueue;
    }

    public void SaveQuestPlan(int questID)
    {
        questPlans.Add(questID, new Queue<GOAP_Action>(currentActions));
        Debug.Log("<color=#0000cc>" + character.characterName + "</color> Saved a new QuestPlan. It includes " + questPlans.Last().Value.Count + " actions and starts with " + questPlans.Last().Value.Peek());
        currentActions.Clear();
        activeAction = null;
        activeGoal.Clear();
    }

    public void Called(Vector3 callerPosition)
    {
        ChangeState(FSM_State.WAITFORCALL);
        View.TurnTo(callerPosition);
    }

    public void ReceiveQuestCompletion(int questID)
    { 
        if(currentState == FSM_State.WAITFORCALL)
        {
            currentState = stateBeforeWait;
        }
        GOAP_QuestBoard.instance.CompleteQuest(questID);
    }

    public bool IsSatisfiedInCurrentWorldstate(List_GOAP_Worldstate worldstates)
    {
        for(int i = 0; i < worldstates.Count; i++)
        {
            if (!currentWorldstates.ContainsExactly(worldstates[i]))
            {
                return false;
            }
        }
        return true;
    }

    public string PrintActionQueue()
    {
        string s = "";
        
        foreach(GOAP_Action action in currentActions)
        {
            s += " -> " + action.ActionID;
        }

        s += " |";

        return s;
    }

    public string PrintCurrentState()
    {
        return currentState.ToString();
    }
}