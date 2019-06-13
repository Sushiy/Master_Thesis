using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlannableActions
{
    None = 0,
    ChopTree = 1 << 0,
    ChopWood = 1 << 1,
    GetAxe = 1 << 2,
    MakeAxe = 1 << 3,
    MakeBread = 1 << 4,
    BuyItem = 1 << 5,
    MineIron = 1 << 6,
    MakePickaxe = 1 << 7,
    Farm = 1 << 8,
    MakeFlour = 1 << 9,
}

public class GOAP_Planner : MonoBehaviour
{
    public static GOAP_Planner instance;
    [HideInInspector]
    public PlannableActions globalKnowledgePlannableActions;
    private HashSet<GOAP_Action> globalKnowledgeAvailableActions;
    public float heuristicFactor = 2f;

    public bool writePlannerLog = true;
    string plannerLog = "";

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        globalKnowledgeAvailableActions = new HashSet<GOAP_Action>();

        GetActionSet(this.globalKnowledgePlannableActions, ref globalKnowledgeAvailableActions);
    }

    public void InstantiateAction<T>(PlannableActions plannableActions, ref HashSet<GOAP_Action> set) where T : GOAP_Action, new()
    {
        T action = new T();
        if (IsActionAvailable(plannableActions, (PlannableActions)Enum.Parse(typeof(PlannableActions), action.ActionID)))
        {
            set.Add(action);
        }
    }

    public void InstantiateBaseAction<T>(ref HashSet<GOAP_Action> set) where T : GOAP_Action, new()
    {
        set.Add(new T());
    }

    public void GetActionSet(PlannableActions plannableActions, ref HashSet<GOAP_Action> set)
    {
        //BASE ACTION
        InstantiateBaseAction<Action_EatFood>(ref set);
        InstantiateBaseAction<Action_GatherFirewood>(ref set);
        InstantiateBaseAction<Action_Sleep>(ref set);
        InstantiateBaseAction<Action_GetWater>(ref set);
        InstantiateBaseAction<Action_CheckForQuest>(ref set);

        //Extra Actions
        InstantiateAction<Action_ChopTree>(plannableActions, ref set);
        InstantiateAction<Action_ChopWood>(plannableActions, ref set);
        InstantiateAction<Action_GetAxe>(plannableActions, ref set);
        InstantiateAction<Action_MakeAxe>(plannableActions, ref set);
        InstantiateAction<Action_MakeBread>(plannableActions, ref set);
        InstantiateAction<Action_BuyItem>(plannableActions, ref set);
        InstantiateAction<Action_MineIron>(plannableActions, ref set);
        InstantiateAction<Action_MakePickaxe>(plannableActions, ref set);
        InstantiateAction<Action_Farm>(plannableActions, ref set);
        InstantiateAction<Action_MakeFlour>(plannableActions, ref set);


        string msg = "<b>Initializing ActionSet \nAvailable Actions:</b>\n";
        foreach(GOAP_Action action in set)
        {
            msg += action.ActionID + "\n";
        }
        Debug.Log(msg);
    }

    public bool IsActionAvailable(PlannableActions plannableActions, PlannableActions action)
    {
        return (plannableActions & action) != PlannableActions.None;
    }

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, List<GOAP_Worldstate> goal, HashSet<GOAP_Worldstate> currentWorldState, HashSet<GOAP_Action> availableActions)
    {
        plannerLog = "<color=#0000cc> <b>PLANNING</b>: " + agent.Character.characterName + "</color>\n";

        //Search for a valid plan
        Node startNode = WhileBuild(goal, new List<GOAP_Action>(availableActions), currentWorldState, agent);
        plannerLog += "\n";

        //Return null if you couldn't find a plan!
        if (startNode == null)
        {
            plannerLog += "<color=#cc0000>Couldn't find actions fulfilling " + agent.Character.characterName + "s goal.</color>\n";

            Debug.Log(plannerLog);
            return null;
        }
        //Also return null, if the startnode is the goalNode
        if (startNode.action == null && agent.activeQuest == null)
        {
            plannerLog += "Plan has already been fulfilled.\n";

            Debug.Log(plannerLog);
            return null;
        }
        plannerLog += "<color=#00cc00>" + agent.Character.characterName + "found plan:</color>\n";

        //Otherwise return the queue
        return MakeQueue(startNode, agent);
    }

    //Get the agents goal and try to find a plan for it
    public Queue<GOAP_Action> Plan(GOAP_Agent agent, List<GOAP_Worldstate> goal, HashSet<GOAP_Worldstate> currentWorldState)
    {
        return Plan(agent, goal, currentWorldState, globalKnowledgeAvailableActions);
    }

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, List<GOAP_Worldstate> goal, HashSet<GOAP_Worldstate> currentWorldState, PlannableActions plannableActions)
    {
        HashSet<GOAP_Action> availableActions = new HashSet<GOAP_Action>();
        GetActionSet(plannableActions, ref availableActions);
        return Plan(agent, goal, currentWorldState, availableActions);
    }

    //Perform A* reverse pathfinding search to get a plan
    private Node WhileBuild(List<GOAP_Worldstate> goal, List<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState, GOAP_Agent agent)
    {
        Node current = null;

        HashSet<Node> closedSet = new HashSet<Node>(); //Hashset for performance and uniqueness

        List<Node> openSet = new List<Node>(); //This is a List so it can be sorted

        Node goalNode = GetGoalNode(currentWorldState, goal);
        //If the goal is already fulfilled, return the goal
        if(goalNode.required.Count == 0)
        {
            plannerLog += "Goal was already fulfilled\n";
            return goalNode;
        }

        plannerLog += "Starting reverse planning: \n\n";
        //Add the goal as first node
        openSet.Add(goalNode);


        int graphDepth = 0;
        //Reverse A*
        while(openSet.Count > 0)
        {
            openSet.Sort();
            current = openSet[0];

            //Debug Log to visualize the process
            string msg = "";
            if (writePlannerLog)
            {
                foreach (GOAP_Worldstate state in current.required)
                {
                    msg += state.ToString() + ",";
                }
                if (current.isSkilled)
                    plannerLog += makeIndent(graphDepth) + "-><color=#00CC00>ClosedSet Updated</color> (" + current.estimatedPathCost + "); ";
                else
                    plannerLog += makeIndent(graphDepth) + "-><color=#0000CC>ClosedSet Updated</color> (" + current.estimatedPathCost + "); ";
                if (msg.Equals("")) msg = "empty";
                plannerLog += "(" + msg + ")";
                if (current.action != null) plannerLog += "Action: " + current.action.ActionID;
                plannerLog += "\n";
            }

            if (current.required.Count == 0)
            {
                return current;
            }
            openSet.Remove(current);
            closedSet.Add(current);
            bool foundValidNeighbor = false;
            for (int i = 0; i < availableActions.Count; i++)
            {
                if (availableActions[i].Equals(current.action)) continue; //Dont do the same action twice

                Node neighbor = (availableActions[i].ActionID == "BuyItem") ? GenerateBuyNode(current, currentWorldState,agent) : GetValidNeighborNode(current, availableActions[i], currentWorldState, agent);
                if(neighbor != null)
                {
                    foundValidNeighbor = true;
                    int indexOfSameState = openSet.IndexOf(neighbor);
                    if (indexOfSameState != -1)
                    {
                        if (openSet[indexOfSameState].estimatedPathCost > neighbor.estimatedPathCost)
                        {
                            
                            openSet.Remove(openSet[indexOfSameState]);
                            
                            openSet.Add(neighbor);
                            if (writePlannerLog)
                            {
                                msg = "";
                                foreach (GOAP_Worldstate state in neighbor.required)
                                {
                                    msg += state.ToString() + ",";
                                }
                                if (neighbor.isSkilled)
                                    plannerLog += makeIndent(graphDepth) + "-><color=#CCCC00>OpenSet Replaced</color> (" + neighbor.estimatedPathCost + "); ";
                                else
                                    plannerLog += makeIndent(graphDepth) + "-><color=#00CCCC>OpenSet Replaced</color> (" + neighbor.estimatedPathCost + "); ";
                                if (msg.Equals("")) msg = "empty";
                                plannerLog += "(" + msg + ") ";
                                if (neighbor.action != null) plannerLog += "Action: " + neighbor.action.ActionID;
                                plannerLog += "\n";
                            }
                        }
                        else
                        {
                            if (writePlannerLog)
                            {
                                msg = "";
                                foreach (GOAP_Worldstate state in neighbor.required)
                                {
                                    msg += state.ToString() + ",";
                                }
                                if (neighbor.isSkilled)
                                    plannerLog += makeIndent(graphDepth) + "-><color=#CC0000>OpenSet Not Replaced</color> (" + neighbor.estimatedPathCost + "); ";
                                else
                                    plannerLog += makeIndent(graphDepth) + "-><color=#00CCCC>OpenSet Not Replaced</color> (" + neighbor.estimatedPathCost + "); ";
                                if (msg.Equals("")) msg = "empty";
                                plannerLog += "(" + msg + ") ";
                                if (neighbor.action != null) plannerLog += "Action: " + neighbor.action.ActionID;
                                plannerLog += "\n";
                            }
                        }
                    }
                    else
                    {
                        openSet.Add(neighbor);
                        //Debug Log to visualize the process
                        if (writePlannerLog)
                        {
                            msg = "";
                            foreach (GOAP_Worldstate state in neighbor.required)
                            {
                                msg += state.ToString() + ",";
                            }
                            if (neighbor.isSkilled)
                                plannerLog += makeIndent(graphDepth) + "-><color=#CCCC00>OpenSet Updated</color> (" + neighbor.estimatedPathCost + "); ";
                            else
                                plannerLog += makeIndent(graphDepth) + "-><color=#00CCCC>OpenSet Updated</color> (" + neighbor.estimatedPathCost + "); ";
                            if (msg.Equals("")) msg = "empty";
                            plannerLog += "(" + msg + ") ";
                            if (neighbor.action != null) plannerLog += "Action: " + neighbor.action.ActionID;
                            plannerLog += "\n";
                        }
                    }
                }
            }
            if(!foundValidNeighbor  && agent.activeQuest == null)
            {
                Node questNode = GenerateQuestNode(current, currentWorldState, agent);
                openSet.Add(questNode);
                //Debug Log to visualize the process
                if (writePlannerLog)
                {
                    msg = "";
                    foreach (GOAP_Worldstate state in questNode.required)
                    {
                        msg += state.ToString() + ",";
                    }
                    plannerLog += makeIndent(graphDepth) + "-><color=#660000>OpenSet Updated</color> (" + questNode.estimatedPathCost + "); ";
                    if (msg.Equals("")) msg = "empty";
                    plannerLog += "(" + msg + ") ";
                    if (questNode.action != null) plannerLog += "Action: " + questNode.action.ActionID;
                    plannerLog += "\n";
                }
            }
            graphDepth++;

        }

        return null;
    }

    public bool IsGoalSatisfied(HashSet<GOAP_Worldstate> currentWorldState, List<GOAP_Worldstate> goalWorldState)
    {
        //First, check if we have not already reached the goal, by checking it against our currentWorldstate

        foreach (GOAP_Worldstate state in goalWorldState)
        {
            if (!currentWorldState.Contains(state))
            {
                return false;
            }
        }
        return true;
    }

    //Combine Current and Goal Worldstate to see if a plan needs to be made in order to fulfill this goal
    private Node GetGoalNode(HashSet<GOAP_Worldstate> currentWorldState, List<GOAP_Worldstate> goalWorldState)
    {
        HashSet<GOAP_Worldstate> newRequired = new HashSet<GOAP_Worldstate>(goalWorldState);
        plannerLog += "Found Goal:\n";
        foreach (GOAP_Worldstate state in goalWorldState)
        {
            plannerLog += state.ToString() + "\n";
            if (currentWorldState.Contains(state))
            {
                newRequired.Remove(state);
            }
        }
        plannerLog += "\n";
        return new Node(null, newRequired, null, 0, true);
    }

    //Try to apply the action onto the activeNode to see if it results in a valid neighbor
    private Node GetValidNeighborNode(Node activeNode, GOAP_Action action, HashSet<GOAP_Worldstate> planningWorldState, GOAP_Agent agent)
    {
        bool isValidAction = false;

        HashSet<GOAP_Worldstate> newRequired = new HashSet<GOAP_Worldstate>(activeNode.required);
        //Actions need to fulfill at least one required Worldstate to result in a valid neighbor
        foreach (GOAP_Worldstate state in activeNode.required)
        {
            if (action.SatisfyWorldstates.Contains(state))
            {
                newRequired.Remove(state);
                isValidAction = true;
            }
        }

        if (!isValidAction) return null;
        //If the actions proceduralConditions are not met, we can't perform it anyways
        if (!action.CheckProceduralConditions(agent)) return null;

        //add the actions own required worldstates to the Node
        foreach (GOAP_Worldstate state in action.RequiredWorldstates)
        {
            if (!planningWorldState.Contains(state))
            {
                newRequired.Add(state);
            }
        }

        //Apply skillmodification onto the neighbor if it is valid
        float skillModifier = 1f;
        bool isSkilled = true;
        if (action.RequiredSkill != null)
        {
            int index = agent.Character.skills.IndexOf(action.RequiredSkill);
            if (index != -1)
            {
                //If the character is actually skilled in this action, adjust the skillmodifier
                int difference = action.RequiredSkill.level - agent.Character.skills[index].level;
                if (difference > 0) skillModifier *= difference + 1;
                else skillModifier /= (-difference) + 1;
            }
            else
            {
                //If the character is not skilled in this action, the skillmodifier is set to 5. This only comes into play, when global knowledge planning is used.
                isSkilled = false;
                skillModifier = 5f;
            }
        }

        //Change the skillmodifier on the action 
        action.ApplySkillModifier(skillModifier);

        return new Node(activeNode, newRequired, action, newRequired.Count * heuristicFactor + action.ActionCost + activeNode.estimatedPathCost, isSkilled);
    }

    private Node GenerateBuyNode(Node activeNode, HashSet<GOAP_Worldstate> planningWorldState, GOAP_Agent agent)
    {
        HashSet<GOAP_Worldstate> newRequired = new HashSet<GOAP_Worldstate>(activeNode.required);
        Action_BuyItem action = new Action_BuyItem();
        action.CheckProceduralConditions(agent);

        bool isValidAction = false;

        //Check for eHasItem worldStates
        foreach (GOAP_Worldstate state in activeNode.required)
        {
            if (state.key == WorldStateKey.eHasItem)
            {
                action.SetWantedItem((ItemType)state.value);
                newRequired.Remove(state);
                isValidAction = true;
                break;
            }
        }

        if (!isValidAction) return null;

        float estimatedBuyCost = action.ActionCost * activeNode.required.Count;
        return new Node(activeNode, newRequired, action, estimatedBuyCost + activeNode.estimatedPathCost, true);
    }

    //Generate a quest for the current Node, because it is somehow unsolvable
    private Node GenerateQuestNode(Node activeNode, HashSet<GOAP_Worldstate> planningWorldState, GOAP_Agent agent)
    {
        HashSet<GOAP_Worldstate> newRequired = new HashSet<GOAP_Worldstate>();
        Action_PostQuest action = new Action_PostQuest();
        action.CheckProceduralConditions(agent);
        foreach(GOAP_Worldstate state in activeNode.required)
        {
            Debug.Log("Adding state " + state.ToString() + " to quest");
            action.AddQuestWorldstate(state);
        }
        float estimatedQuestCost = action.ActionCost * activeNode.required.Count;
        return new Node(activeNode, newRequired, action, estimatedQuestCost + activeNode.estimatedPathCost, true);
    }

    //Form a queue of actions from the plan of nodes
    private Queue<GOAP_Action> MakeQueue(Node start, GOAP_Agent agent)
    {
        Queue<GOAP_Action> queue = new Queue<GOAP_Action>();
        string message = "<color=#00AA00>ActionQueue:</color> ";
        Node current = start;
        bool needsQuest = false;
        QuestData questData = new QuestData(agent);

        while (current.parent != null)
        {
            if(current.isSkilled)
            {
                queue.Enqueue(current.action);
                message += " -> " + current.action.ActionID;
                if (current.action.ActionID == "PostQuest")
                {
                    Action_WaitForQuest waitForQuest = new Action_WaitForQuest();
                    queue.Enqueue(waitForQuest);
                    message += " -> " + waitForQuest.ActionID;
                }

                if (!needsQuest)
                {
                    questData.ClearProvided();
                    foreach (GOAP_Worldstate state in current.action.SatisfyWorldstates)
                    {
                        questData.AddProvided(state);
                    }
                }          
            }
            //Generate Quest instead of Actions the agent is unskilled with
            else
            {
                queue.Clear();
                queue.Enqueue(new Action_PostQuest());
                message += " -> <color=#CC0000> QUEST: " + current.action.ActionID + "</color>";
                questData.ClearRequired();
                foreach (GOAP_Worldstate state in current.action.SatisfyWorldstates)
                {
                    questData.AddRequired(state);
                }
                needsQuest = true;
            }

            current = current.parent;
        }

        if(agent.activeQuest != null)
        {
            Action_CompleteQuest completeQuest = new Action_CompleteQuest();
            completeQuest.SetActionTarget(agent.activeQuest.Owner);
            queue.Enqueue(completeQuest);
            message += " -> " + completeQuest.ActionID;
        }

        message += "|";
        plannerLog += message;
        Debug.Log(plannerLog);

        if (needsQuest)
        {
            agent.postedQuest = new GOAP_Quest(questData);
            GOAP_QuestBoard.instance.AddQuest(agent.postedQuest);
        }
        return queue;
    }

    string makeIndent(int depth)
    {
        string s = "";
        for(int i = 0; i<depth; i++)
        {
            s += "  ";
        }
        return s;
    }
    
    private class Node : System.IComparable<Node>, System.IEquatable<Node>
    {
        public Node parent;
        public float estimatedPathCost;
        public HashSet<GOAP_Worldstate> required;
        public GOAP_Action action;
        public bool isSkilled;

        public Node(Node parent, HashSet<GOAP_Worldstate> required, GOAP_Action action, float estimatedPathCost, bool isSkilled)
        {
            this.parent = parent;
            this.estimatedPathCost = estimatedPathCost;
            this.required = required;
            this.action = action;
            this.isSkilled = isSkilled;
        }

        public Node GetStartOfPath()
        {
            if (parent == null)
                return this;
            else
                return parent;
        }

        /***** Interface Methods *****/

        public int CompareTo(Node other)
        {
            return this.estimatedPathCost.CompareTo(other.estimatedPathCost);
        }

        public bool Equals(Node other)
        {
            bool sameRequiredStates = true;
            foreach(GOAP_Worldstate state in required)
            {
                if(!other.required.Contains(state))
                {
                    sameRequiredStates = false;
                    break;
                }
            }
            return sameRequiredStates;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Node objectToCompareWith = (Node)obj;
            return Equals(objectToCompareWith);
        }

        public override int GetHashCode()
        {
            int calculation = 0;
            foreach(GOAP_Worldstate state in required)
            {
                calculation += state.GetHashCode();
            }
            if (action != null)
                calculation += action.ActionID.GetHashCode();
            return calculation ;
        }
    }
}

