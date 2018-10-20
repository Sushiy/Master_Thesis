using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Planner : MonoBehaviour
{

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, HashSet<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState)
    {
        Debug.Log("Start Plan");
        int actionCount = availableActions.Count;
        Queue<GOAP_Action> plan = new Queue<GOAP_Action>();

        /***** GET ACTIVE GOAL *****/
        HashSet<GOAP_Worldstate> goal = new HashSet<GOAP_Worldstate>();// = agent.getCurrentGoal();
        goal.Add(new GOAP_Worldstate(WorldStateKey.bHasWood, true, null));

        /***** SEARCH FOR A VALID PLAN *****/

        Node n = buildGraph(goal, new List<GOAP_Action>(availableActions), currentWorldState, null);

        //Return null if you couldn't find a plan!
        if (n == null)
        {
            Debug.Log("Couldn't find actions fulfilling this goal");
            return null;
        }
        //Otherwise return the queue
        return makeQueue(n);
    }

    private Node buildGraph(HashSet<GOAP_Worldstate> requiredWorldState, List<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState, Node parent)
    {
        //How many worldstates do we still need to fulfill?
        int requiredWorldStateCount = requiredWorldState.Count;

        //How many actions are available
        int actionCount = availableActions.Count;

        List<Node> closedSet = new List<Node>();
        List<GOAP_Action> openSet = new List<GOAP_Action>(availableActions);
        
        Node bestNode = null;

        for(int i = 0; i < actionCount; i++)
        {
            GOAP_Action action = availableActions[i];
            Node tmp = GetPlannerNode(requiredWorldState, currentWorldState, action, parent);
            if (tmp != null)
            {
                //string msg = "";
                //foreach (GOAP_Worldstate state in action.RequiredWorldstates)
                //{
                //    msg += state.key.ToString() + ",";
                //}
                //Debug.Log("Checked " + action.ActionID + "(" + tmp.estimatedPathCost + "); Requires: " + msg);
                closedSet.Add(tmp);
                openSet.Remove(action);
            }
        }
        closedSet.Sort();
        //if at least one node was found

        while (closedSet.Count > 0)
        {
            bestNode = closedSet[0];
            if (bestNode != null)
            {
                //We need to keep looking for more nodes
                if (bestNode.newRequired.Count > 0)
                {
                    string msg = "";
                    foreach(GOAP_Worldstate state in bestNode.newRequired)
                    {
                        msg += state.key.ToString() + ",";
                    }
                    Debug.Log("Chose " + bestNode.action.ActionID + "; Queue still requires: " + msg);
                    Node next = buildGraph(bestNode.newRequired, openSet, currentWorldState, bestNode);
                    if (next == null)
                    {
                        Debug.Log(bestNode.action.ActionID + " cannot fulfill the goal");
                        closedSet.Remove(bestNode);
                        openSet.Add(bestNode.action);
                        continue;
                    }
                    else
                    {
                        return next;
                    }
                }
                else
                {
                    Debug.Log("Chose " + bestNode.action.ActionID + "; Found a Path to goal!");
                    return bestNode;
                }
            }
        }

        return null;
    }

    private Queue<GOAP_Action> makeQueue(Node start)
    {
        Queue<GOAP_Action> queue = new Queue<GOAP_Action>();
        string message = "ActionQueue: ";
        Node current = start;
        while(current != null)
        {
            queue.Enqueue(current.action);
            message += " -> " + current.action.ActionID;
            current = current.parent;
        }
        message += "|";
        Debug.Log(message);
        return queue;
    }

    //Compare these Worldstates to determine if they match up
    private Node GetPlannerNode(HashSet<GOAP_Worldstate> required, HashSet<GOAP_Worldstate> current, GOAP_Action action, Node parent)
    {
        bool isValidAction = false;
        HashSet<GOAP_Worldstate> newRequired = new HashSet<GOAP_Worldstate>(required);
        foreach(GOAP_Worldstate state in required)
        {
            if(action.SatisfyWorldStates.Contains(state))
            {
                newRequired.Remove(state);
                isValidAction = true;
            }
        }

        //add the actions own required worldstates to the Node
        foreach(GOAP_Worldstate state in action.RequiredWorldstates)
        {
            if(!current.Contains(state))
            {
                newRequired.Add(state);
            }
        }

        //Debug.Log(action.ActionID + " isValidAction? " + isValidAction);

        return isValidAction ? new Node(parent, newRequired, action) : null;
    }

    private class Node : System.IComparable<Node>
    {
        public Node parent;
        public float estimatedPathCost;
        public HashSet<GOAP_Worldstate> newRequired;
        public GOAP_Action action;

        public Node(Node parent, HashSet<GOAP_Worldstate> newRequired, GOAP_Action action)
        {
            this.parent = parent;
            this.estimatedPathCost = newRequired.Count + action.ActionCost;
            if (parent != null)
                this.estimatedPathCost += parent.estimatedPathCost;
            this.newRequired = newRequired;
            this.action = action;
        }

        public int CompareTo(Node other)
        {
            return this.estimatedPathCost.CompareTo(other.estimatedPathCost);
        }

        public Node GetStartOfPath()
        {
            if (parent == null)
                return this;
            else
                return parent;
        }
    }
}

