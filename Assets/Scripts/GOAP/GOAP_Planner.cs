using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlannableActions
{
    None = 0,
    ChopTree = 1 << 0,
    ChopWood = 1 << 1,
    GatherFirewood = 1 << 2,
    GetAxe = 1 << 3,
    MakeAxe = 1 << 4,
    MakeBread = 1 << 5,
    BuyItem = 1 << 6,
    MineIron = 1 << 7,
}

public class GOAP_Planner : MonoBehaviour
{
    public static GOAP_Planner instance;
    [HideInInspector]
    public PlannableActions globalKnowledgePlannableActions;
    private HashSet<GOAP_Action> globalKnowledgeAvailableActions;

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

    public void GetActionSet(PlannableActions plannableActions, ref HashSet<GOAP_Action> set)
    {
        if (IsActionAvailable(plannableActions, PlannableActions.ChopTree))
        {
            set.Add(new Action_ChopTree());
        }
        if (IsActionAvailable(plannableActions, PlannableActions.ChopWood))
        {
            set.Add(new Action_ChopWood());
        }
        if (IsActionAvailable(plannableActions, PlannableActions.GatherFirewood))
        {
            set.Add(new Action_GatherFirewood());
        }
        if (IsActionAvailable(plannableActions, PlannableActions.GetAxe))
        {
            set.Add(new Action_GetAxe());
        }
        if (IsActionAvailable(plannableActions, PlannableActions.MakeAxe))
        {
            set.Add(new Action_MakeAxe());
        }
        if (IsActionAvailable(plannableActions, PlannableActions.MakeBread))
        {
            set.Add(new Action_BakeBread());
        }
        if (IsActionAvailable(plannableActions, PlannableActions.BuyItem))
        {
            set.Add(new Action_BuyItem());
        }
        if (IsActionAvailable(plannableActions, PlannableActions.MineIron))
        {
            set.Add(new Action_MineIron());
        }
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

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, HashSet<GOAP_Worldstate> goal, HashSet<GOAP_Worldstate> currentWorldState, HashSet<GOAP_Action> availableActions)
    {
        Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> started planning.");
        plannerLog = "";

        //Search for a valid plan
        Node startingNode = WhileBuild(goal, new List<GOAP_Action>(availableActions), currentWorldState, agent);

        //Return null if you couldn't find a plan!
        if (startingNode == null)
        {
            Debug.Log("<color=#ff0000>Couldn't find actions fulfilling " + agent.Character.characterName + "s goal.</color>");
            return null;
        }
        Debug.Log(plannerLog);

        //Otherwise return the queue
        return MakeQueue(startingNode, agent);
    }

    //Get the agents goal and try to find a plan for it
    public Queue<GOAP_Action> Plan(GOAP_Agent agent, HashSet<GOAP_Worldstate> goal, HashSet<GOAP_Worldstate> currentWorldState)
    {
        return Plan(agent, goal, currentWorldState, globalKnowledgeAvailableActions);
    }

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, HashSet<GOAP_Worldstate> goal, HashSet<GOAP_Worldstate> currentWorldState, PlannableActions plannableActions)
    {
        HashSet<GOAP_Action> availableActions = new HashSet<GOAP_Action>();
        GetActionSet(plannableActions, ref availableActions);
        return Plan(agent, goal, currentWorldState, availableActions);
    }

    //Perform A* reverse pathfinding search to get a plan
    private Node WhileBuild(HashSet<GOAP_Worldstate> goalWorldState, List<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState, GOAP_Agent agent)
    {
        Node current = null;

        HashSet<Node> closedSet = new HashSet<Node>(); //Hashset for performance and uniqueness

        List<Node> openSet = new List<Node>(); //This is a List so it can be sorted

        //First, check if we have not already reached the goal, by checking it against our currentWorldstate
        Node start = GetStartNode(currentWorldState, goalWorldState);

        if(start.required.Count == 0)
        {
            Debug.Log("<color=#00cc00>Goal is already reached. No actions necessary.</color>");
            return start;
        }

        //Add the goal as first node
        openSet.Add(start);

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
                    msg += state.key.ToString() + ",";
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
                if(neighbor != null && !openSet.Contains(neighbor))
                {
                    foundValidNeighbor = true;
                    openSet.Add(neighbor);
                    //Debug Log to visualize the process
                    if (writePlannerLog)
                    {
                        msg = "";
                        foreach (GOAP_Worldstate state in neighbor.required)
                        {
                            msg += state.key.ToString() + ",";
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
            if(!foundValidNeighbor && agent.activeQuest == null)
            {
                plannerLog += makeIndent(graphDepth) + "-><color=#CC0000>Posting Quest.</color>\n";
                Node questNode = GenerateQuestNode(current, currentWorldState, agent);
                openSet.Add(questNode);
                //Debug Log to visualize the process
                if (writePlannerLog)
                {
                    msg = "Quest:";
                    foreach (GOAP_Worldstate state in questNode.required)
                    {
                        msg += state.key.ToString() + ",";
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

    //Combine Current and Goal Worldstate to see if a plan needs to be made in order to fulfill this goal
    private Node GetStartNode(HashSet<GOAP_Worldstate> currentWorldState, HashSet<GOAP_Worldstate> goalWorldState)
    {
        HashSet<GOAP_Worldstate> newRequired = new HashSet<GOAP_Worldstate>(goalWorldState);
        string msg = "StartNode:";
        foreach (GOAP_Worldstate state in goalWorldState)
        {
            msg += "\n" + state.ToString() + ":";
            if (currentWorldState.Contains(state))
            {
                newRequired.Remove(state);
                msg += " already satisfied";
            }
        }
        Debug.Log(msg);
        return new Node(null, newRequired, null, 0, true);
    }

    //Try to apply the action onto the activeNode to see if it results in a valid neighbor
    private Node GetValidNeighborNode(Node activeNode, GOAP_Action action, HashSet<GOAP_Worldstate> currentWorldState, GOAP_Agent agent)
    {
        bool isValidAction = false;

        //If the actions proceduralConditions are not met, we can't perform it anyways
        if (!action.CheckProceduralConditions(agent)) return null;

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

        //add the actions own required worldstates to the Node
        foreach (GOAP_Worldstate state in action.RequiredWorldstates)
        {
            if (!currentWorldState.Contains(state))
            {
                newRequired.Add(state);
            }
        }

        if (!isValidAction) return null;
        
        //Apply skillmodification onto the neighbor if it is valid
        float skillModifier = 1f;
        bool isSkilled = true;
        if (action.RequiredSkill != null)
        {
            int index = agent.Character.skills.IndexOf(action.RequiredSkill);
            if (index != -1)
            {
                int difference = action.RequiredSkill.level - agent.Character.skills[index].level;
                if (difference > 0) skillModifier *= difference + 1;
                else skillModifier /= (-difference) + 1;
            }
            else
            {
                isSkilled = false;
                skillModifier = 5f;
            }
        }

        return new Node(activeNode, newRequired, action, newRequired.Count + action.ActionCost * skillModifier + activeNode.estimatedPathCost, isSkilled);
    }

    private Node GenerateBuyNode(Node activeNode, HashSet<GOAP_Worldstate> currentWorldState, GOAP_Agent agent)
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
                action.SetWantedItem((ItemIds)state.value);
                newRequired.Remove(state);
                isValidAction = true;
                break;
            }
        }

        if (!isValidAction) return null;

        float estimatedQuestCost = action.ActionCost * activeNode.required.Count;
        return new Node(activeNode, newRequired, action, estimatedQuestCost + activeNode.estimatedPathCost, true);
    }

    //Generate a quest for the current Node, because it is somehow unsolvable
    private Node GenerateQuestNode(Node activeNode, HashSet<GOAP_Worldstate> currentWorldState, GOAP_Agent agent)
    {
        HashSet<GOAP_Worldstate> newRequired = new HashSet<GOAP_Worldstate>();
        Action_PostQuest action = new Action_PostQuest();
        action.CheckProceduralConditions(agent);
        foreach(GOAP_Worldstate state in activeNode.required)
        {
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
        GOAP_Quest quest = new GOAP_Quest(agent);

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
                    quest.ClearProvided();
                    foreach (GOAP_Worldstate state in current.action.SatisfyWorldstates)
                    {
                        quest.AddProvided(state);
                    }
                }          
            }
            //Generate Quest instead of Actions the agent is unskilled with
            else
            {
                queue.Clear();
                queue.Enqueue(new Action_PostQuest());
                message += " -> <color=#CC0000> QUEST: " + current.action.ActionID + "</color>";
                quest.ClearRequired();
                foreach (GOAP_Worldstate state in current.action.SatisfyWorldstates)
                {
                    quest.AddRequired(state);
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
        Debug.Log(message);

        if (needsQuest)
        {
            agent.postedQuest = quest;
            GOAP_QuestBoard.instance.AddQuest(quest);
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
    
    private class Node : System.IComparable<Node>, System.IEquatable<GOAP_Worldstate>
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

        public bool Equals(GOAP_Worldstate other)
        {
            return Equals(other, this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Node objectToCompareWith = (Node)obj;
            if (objectToCompareWith.action == null)
            {
                return (action == null);
            }
            return objectToCompareWith.GetHashCode().Equals(GetHashCode()) && objectToCompareWith.action.Equals(action);
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

