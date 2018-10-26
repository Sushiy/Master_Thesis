using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Planner : MonoBehaviour
{
    string plannerLog = "";

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, HashSet<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState)
    {
        Debug.Log("Start Plan");
        plannerLog = "";
        int actionCount = availableActions.Count;
        Queue<GOAP_Action> plan = new Queue<GOAP_Action>();

        /***** GET ACTIVE GOAL *****/
        HashSet<GOAP_Worldstate> goal = new HashSet<GOAP_Worldstate>();// = agent.getCurrentGoal();
        goal.Add(new GOAP_Worldstate(WorldStateKey.bHasWood, true, null));

        /***** SEARCH FOR A VALID PLAN *****/

        //Node n = buildGraph(goal, new List<GOAP_Action>(availableActions), currentWorldState, null, 0);
        Node n = WhileBuild(goal, new List<GOAP_Action>(availableActions), currentWorldState);
        //Return null if you couldn't find a plan!
        if (n == null)
        {
            Debug.Log("<color=#ff0000>Couldn't find actions fulfilling this goal.</color>");
            return null;
        }
        Debug.Log(plannerLog);
        //Otherwise return the queue
        return makeQueue(n);
    }

    private Node WhileBuild(HashSet<GOAP_Worldstate> goalWorldState, List<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState)
    {
        //List of worldstates we have checked before
        //List<GOAP_Worldstate> closedSet = new List<GOAP_Worldstate>();

        //List<GOAP_Worldstate> openSet = new List<GOAP_Worldstate>(requiredWorldState);

        Node current = null;

        HashSet<Node> closedSet = new HashSet<Node>();
        List<Node> openSet = new List<Node>();
        openSet.Add(new Node(null, goalWorldState, null, 0));
        int graphDepth = 0;
        while(openSet.Count > 0)
        {
            openSet.Sort();
            current = openSet[0];
            string msg = "";
            foreach (GOAP_Worldstate state in current.required)
            {
                msg += state.key.ToString() + ",";
            }
            plannerLog += makeIndent(graphDepth) + "-><color=#00CC00>ClosedSet Updated</color> (" + current.estimatedPathCost + "); ";
            if (msg.Equals("")) msg = "empty";
            plannerLog += "(" + msg + ")";
            if (current.action != null) plannerLog += "Action: " + current.action.ActionID;
            plannerLog += "\n";
            if (current.required.Count == 0)
            {
                return current;
            }
            openSet.Remove(current);
            closedSet.Add(current);



            for (int i = 0; i < availableActions.Count; i++)
            {
                if (availableActions[i].ActionID.Equals(current.action)) continue; //Dont do the same action twice

                Node neighbor = GetValidNeighborNode(current, availableActions[i], currentWorldState);
                if(neighbor != null && !openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                    msg = "";
                    foreach (GOAP_Worldstate state in neighbor.required)
                    {
                        msg += state.key.ToString() + ",";
                    }
                    plannerLog += makeIndent(graphDepth) + "-><color=#CCCC00>OpenSet Updated</color> (" + neighbor.estimatedPathCost + "); ";
                    if (msg.Equals("")) msg = "empty";
                    plannerLog += "(" + msg + ") ";
                    if (neighbor.action != null) plannerLog += "Action: " + neighbor.action.ActionID;
                    plannerLog += "\n";
                }
            }
            graphDepth++;

        }

        return null;
    }

    private Node GetValidNeighborNode(Node activeNode, GOAP_Action action, HashSet<GOAP_Worldstate> currentWorldState)
    {
        bool isValidAction = false;
        HashSet<GOAP_Worldstate> newRequired = new HashSet<GOAP_Worldstate>(activeNode.required);
        if (action.ActionID != "PostQuest")
        {
            foreach (GOAP_Worldstate state in activeNode.required)
            {
                if (action.SatisfyWorldStates.Contains(state))
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

        }
        else
        {
            Action_PostQuest postQuest = (Action_PostQuest)action;
            postQuest.SetQuestStates(newRequired);
            newRequired.Clear();
            isValidAction = true;
        }

        //Debug.Log(action.ActionID + " isValidAction? " + isValidAction);

        return isValidAction ? new Node(activeNode, newRequired, action, newRequired.Count + action.ActionCost + activeNode.estimatedPathCost) : null;
    }

    private Queue<GOAP_Action> makeQueue(Node start)
    {
        Queue<GOAP_Action> queue = new Queue<GOAP_Action>();
        string message = "<color=#00AA00>ActionQueue:</color> ";
        Node current = start;
        while (current.parent != null)
        {
            queue.Enqueue(current.action);
            message += " -> " + current.action.ActionID;
            current = current.parent;
        }
        message += "|";
        Debug.Log(message);
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

        public Node(Node parent, HashSet<GOAP_Worldstate> required, GOAP_Action action, float estimatedPathCost)
        {
            this.parent = parent;
            this.estimatedPathCost = estimatedPathCost;
            this.required = required;
            this.action = action;
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

