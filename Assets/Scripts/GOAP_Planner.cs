using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlannableActions
{
    None = 0,
    BuyIron = 1 << 0,
    BuyWood = 1 << 1,
    ChopTree = 1 << 2,
    ChopWood = 1 << 3,
    GatherFirewood = 1 << 4,
    GetAxe = 1 << 5,
    MakeAxe = 1 << 6,
}

public class GOAP_Planner : MonoBehaviour
{
    public static GOAP_Planner instance;
    [HideInInspector]
    public PlannableActions plannableActions;
    private HashSet<GOAP_Action> availableActions;

    public bool writePlannerLog = true;
    string plannerLog = "";

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        availableActions = new HashSet<GOAP_Action>();

        InitActions();
    }

    public void InitActions()
    {

        if (IsActionAvailable(PlannableActions.BuyIron))
        {
            availableActions.Add(new Action_BuyIron());
        }
        if (IsActionAvailable(PlannableActions.BuyWood))
        {
            availableActions.Add(new Action_BuyWood());
        }
        if (IsActionAvailable(PlannableActions.ChopTree))
        {
            availableActions.Add(new Action_ChopTree());
        }
        if (IsActionAvailable(PlannableActions.ChopWood))
        {
            availableActions.Add(new Action_ChopWood());
        }
        if (IsActionAvailable(PlannableActions.GatherFirewood))
        {
            availableActions.Add(new Action_GatherFirewood());
        }
        if (IsActionAvailable(PlannableActions.GetAxe))
        {
            availableActions.Add(new Action_GetAxe());
        }
        if (IsActionAvailable(PlannableActions.MakeAxe))
        {
            availableActions.Add(new Action_MakeAxe());
        }
        string msg = "<b>Initializing Planner \nAvailable Actions:</b>\n";
        foreach(GOAP_Action action in availableActions)
        {
            msg += action.ActionID + "\n";
        }
        Debug.Log(msg);
    }

    public bool IsActionAvailable(PlannableActions action)
    {
        return (plannableActions & action) != PlannableActions.None;
    }

    //Get the agents goal and try to find a plan for it
    public Queue<GOAP_Action> Plan(GOAP_Agent agent, HashSet<GOAP_Worldstate> goal, HashSet<GOAP_Worldstate> currentWorldState)
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
        return makeQueue(startingNode, agent);
    }

    //Perform A* reverse pathfinding search to get a plan
    private Node WhileBuild(HashSet<GOAP_Worldstate> goalWorldState, List<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState, GOAP_Agent agent)
    {
        Node current = null;

        HashSet<Node> closedSet = new HashSet<Node>(); //Hashset for performance and uniqueness

        List<Node> openSet = new List<Node>(); //This is a List so it can be sorted
        
        //Add the goal as first node
        openSet.Add(new Node(null, goalWorldState, null, 0, true));

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

            for (int i = 0; i < availableActions.Count; i++)
            {
                if (availableActions[i].ActionID.Equals(current.action)) continue; //Dont do the same action twice

                Node neighbor = GetValidNeighborNode(current, availableActions[i], currentWorldState, agent);
                if(neighbor != null && !openSet.Contains(neighbor))
                {
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
            graphDepth++;

        }

        return null;
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

    //Form a queue of actions from the plan of nodes
    private Queue<GOAP_Action> makeQueue(Node start, GOAP_Agent agent)
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
                if(!needsQuest)
                {
                    quest.ClearProvided();
                    foreach(GOAP_Worldstate state in current.action.SatisfyWorldstates)
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

