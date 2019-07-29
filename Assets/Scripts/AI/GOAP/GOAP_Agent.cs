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
    public int? activePlanInfo = null;

    public GOAP_Agent(GOAP_Character character, IGOAP_AgentView view)
    {
        this.character = character;
        View = view;

        checkedCharacterGoals = new List_GOAP_Worldstate();

        currentActions = new Queue<GOAP_Action>();
        currentWorldstates = new List_GOAP_Worldstate();
        checkedQuestIds = new List<int>();

        postedQuestIDs = new List<int>();
        completedQuestIDs = new List<int>();

        questPlans = new Dictionary<int, Queue<GOAP_Action>>();
        activeGoal = new List_GOAP_Worldstate();

        planMemory = new List<PlanInfo>();

        timeSincePlanned = Random.Range(0, planningWaitTimer);
    }

    /// <summary>
    /// This action chooses a goal from among the personal goals after checking if there are completed quests to finish first
    /// </summary>
    public void ChooseGoal()
    {
        activeGoal.Clear();
        string msg = "<color=#0000cc><b>CHECKING GOALS</b>:" + character.characterData.characterName + "</color>\n";

        //Check goals top to bottom, to see which need to be fulfilled
        for (int i = 0; i < character.characterData.goals.Count; i++)
        {
            GOAP_Worldstate goal = character.characterData.goals[i];

            //Check if the goal is already satisfied
            if(!GOAP_Planner.IsGoalSatisfied(currentWorldstates, goal))
            {
                //And if it has been checked before
                if(!checkedCharacterGoals.ContainsExactly(goal))
                {
                    checkedCharacterGoals.Add(character.characterData.goals[i]);
                    activeGoal.Add(goal);
                    msg += character.characterData.goals[i].key + ":" + character.characterData.goals[i].value + " not yet satisfied\n";
                    break;
                }
                else
                {
                    msg += character.characterData.goals[i].key + ":" + character.characterData.goals[i].value + " not yet satisfied, but was already checked\n";
                }
            }
            else
            {
                if(checkedCharacterGoals.ContainsExactly(character.characterData.goals[i]))
                {
                    checkedCharacterGoals.Remove(character.characterData.goals[i]);
                }
                msg += character.characterData.goals[i].key + ":" + character.characterData.goals[i].value + " already satisfied\n";
            }
        }
        Character.Log(msg);

        timeSincePlanned = 0.0f;
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

    /// <summary>
    /// Updates current Worldstates, so that observed worldstates can be forgotten
    /// </summary>
    /// <param name="deltaTime"></param>
    private void CurrentWorldstateUpdate(float deltaTime)
    {
        for(int i = currentWorldstates.Count -1; i >= 0; i--)
        {
            if (currentWorldstates[i].Forget(deltaTime))
            {
                Character.Log("<color=#cc00cc>" + Character.characterData.characterName + "</color> forgets state:" + currentWorldstates[i]);
                RemoveCurrentWorldState(currentWorldstates[i]);
            }
        }
    }

    #region Statemachine

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
        CurrentWorldstateUpdate(deltaTime);
        if (AllowedToPlan || completedQuestIDs.Count > 0)
        {
            ChangeState(FSM_State.PLANNING);
            return;
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
                Character.Log("<color=#0000cc>" + character.characterData.characterName + "</color>s Quest " + postedQuestIDs[i] + " was already solved.");
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
            planMemory.Add(new PlanInfo(PrintGoal(), Character.characterData.characterName));
            newPlan = GOAP_Planner.Plan(this, activeGoal, currentWorldstates, character.characterData.availableActions);

            timeSincePlanned = 0.0f;

            if (newPlan != null)
            {
                //do what the plan says!
                currentActions = newPlan;
                actionCompleted = true;
                planMemory[planMemory.Count - 1].ApprovePlan(planMemory.Count - 1, PrintActionQueue());
                activePlanInfo = planMemory.Count - 1;
                if (activePlanInfo == -1) Debug.LogError(character.characterData.characterName + " ActivePlanIndex: -1");
                ChangeState(FSM_State.PERFORMACTION);
                return;
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
                    return;
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
                    currentActions = new Queue<GOAP_Action>(questPlans[id].ToArray());
                    Character.Log("<color=#0000cc>" + character.characterData.characterName + "</color> has completed Quest " + id + " to finish. It includes " + currentActions.Count + " actions and starts with " + currentActions.Peek());

                    questPlans.Remove(id);
                    Character.Log("<color=#0000cc>" + character.characterData.characterName + "</color> removes questplan" + id);
                    ChangeState(FSM_State.PERFORMACTION);
                    return;
                }
                else
                {
                    string msg = "";
                    foreach (int key in questPlans.Keys)
                    {
                        msg += key + ",";
                    }
                    Debug.LogError("<color=#0000cc>" + character.characterData.characterName + "</color> tried to continue Quest " + id + " but didnt find a corresponding plan.\nExisting plans:" + msg);
                    completedQuestIDs.Remove(id);
                    ChangeState(FSM_State.IDLE);
                    return;
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
                    return;
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
            if(activeQuest != null)
            {
                if (!GOAP_QuestBoard.instance.quests.ContainsKey(activeQuest.id))
                {
                    Character.Log("<color=#0000cc>" + Character.characterData.characterName + "</color> can't complete quest, already finished");
                    CancelPlan();
                    return;
                }

            }
            if (activeAction.CheckRequirements(this))
            {
                if (activeAction.SatisfyWorldstates.Count > 0 && IsSatisfiedInCurrentWorldstate(activeAction.SatisfyWorldstates))
                {
                    actionCompleted = true;
                    Character.Log("<color=#0000cc>" + character.characterData.characterName + "</color> didn't need to perform <color=#cc0000>" + activeAction.ActionID + "</color> anymore.");
                }
                else
                {
                    if (!activeAction.IsInRange(this))
                    {
                        View.PrintMessage("MoveTo " + activeAction.ActionID);
                        ChangeState(FSM_State.MOVETO);
                        return;
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
                Character.Log("<color=#0000cc>" + character.characterData.characterName + "</color> cannot perform <color=#cc0000>" + activeAction.ActionID + "</color> anymore.");
                Replan();
            }

        }
        else
        {
            //if this was a personal goal, remove it from the checked list
            if (activeQuest != null)
            {
                activeQuest = null;
            }
            else
            {
                if (activeGoal.Count == 1)
                {

                    checkedCharacterGoals.Remove(activeGoal[0]);
                }
            }
            Character.Log("<color=#0000cc>" + character.characterData.characterName + "</color> has completed his action queue for Goal " + planMemory[(int)activePlanInfo].goalInfo);
            activeGoal.Clear();
            activePlanInfo = null;
            ChangeState(FSM_State.IDLE);
            return;
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
            View.StopMove();
            ChangeState(FSM_State.PERFORMACTION);
            return;
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
    #endregion

    /// <summary>
    ///Cancel the action queue but keep the goal/quest and try planning again immediately
    /// </summary>
    public void Replan()
    {
        activePlanInfo = null;
        activeAction = null;
        currentActions.Clear();
        actionCompleted = true;
        ChangeState(FSM_State.PLANNING);
        return;
    }

    /// <summary>
    /// Cancel the actionqueue and forget the plan
    /// </summary>
    public void CancelPlan()
    {
        if(activeQuest != null)
        {
            checkedQuestIds.Remove(activeQuest.id);
        }
        else
        {
            //if this was a personal goal, remove it from the checked list
            if (activeGoal.Count == 1)
                checkedCharacterGoals.Remove(activeGoal[0]);
        }
        activeQuest = null;
        activeGoal.Clear();
        
        activePlanInfo = null;
        activeAction = null;
        currentActions.Clear();
        actionCompleted = true;
        ChangeState(FSM_State.IDLE);
        return;
    }

    /// <summary>
    /// Checks the questboard for any quests that have not yet been attempted by the agent before
    /// </summary>
    /// <returns></returns>
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
            Debug.Log("<color=#0000cc>" + character.characterData.characterName + "</color> chose to plan for Quest " + result.id);
            checkedQuestIds.Add(result.id);
            activeGoal.AddRange(result.RequiredStates);
        }
        else
        {

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
            if(newState.type == WorldStateType.UNIQUE)
            {
                currentWorldstates.Remove(newState); //this should find the state based on the key only
                currentWorldstates.Add(newState);
                Debug.Log(Character.characterData.characterName + " <color=#cc0000>Updated state:</color> " + newState.ToString());

            }
            else
            {
                if(!currentWorldstates.ContainsExactly(newState))
                {
                    Character.Log(Character.characterData.characterName + " <color=#cc0000>Add state:</color> " + newState.ToString());
                    currentWorldstates.Add(newState);
                }

            }
        }

        //Otherwise, if the newstate is not contained in the currentworldstate, add it
        else
        {
            Character.Log(Character.characterData.characterName + " <color=#cc0000>Add state:</color> " + newState.ToString());
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
            Character.Log(Character.characterData.characterName + " <color=#cc0000>Remove state:</color> " + state.ToString());
            currentWorldstates.Remove(state);
        }
    }

    public void ResetPlanningTimer()
    {
        timeSincePlanned = 0f;
    }

    public void UpdateActionQueue(Queue<GOAP_Action> actionQueue)
    {
        //TODO: I would like to enqueue the whole queue instead of replacing it
        while(actionQueue.Count > 0)
        {
            currentActions.Enqueue(actionQueue.Dequeue());
        }
    }

    public void SaveQuestPlan(int questID)
    {
        questPlans.Add(questID, new Queue<GOAP_Action>(currentActions));
        Character.Log("<color=#0000cc>" + character.characterData.characterName + "</color> Saved a new QuestPlan. It includes " + questPlans.Last().Value.Count + " actions and starts with " + questPlans.Last().Value.Peek());
        currentActions.Clear();
        activeAction = null;
        activeGoal.Clear();
    }

    public void Called(Vector3 callerPosition)
    {
        View.TurnTo(callerPosition);
        ChangeState(FSM_State.WAITFORCALL);
        return;
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

    public string PrintGoal()
    {
        string msg = "";
        if (activeGoal.Count < 1) return "None";
        foreach (GOAP_Worldstate state in activeGoal)
        {
            msg += state.ToString() + "\n";
        }
        if (activeQuest != null)
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
        string msg = Character.characterData.characterName + " Current Worldstates\n";
        foreach (GOAP_Worldstate state in currentWorldstates)
        {
            msg += state.ToString() + "\n";
        }

        return msg;
    }
}