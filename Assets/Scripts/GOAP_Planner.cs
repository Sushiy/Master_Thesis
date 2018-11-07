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
        Node n = WhileBuild(goal, new List<GOAP_Action>(availableActions), currentWorldState, agent);

        //Return null if you couldn't find a plan!
        if (n == null)
        {
            Debug.Log("<color=#ff0000>Couldn't find actions fulfilling this goal.</color>");
            return null;
        }
        Debug.Log(plannerLog);
        //Otherwise return the queue
        return makeQueue(n, agent);
    }

    private Node WhileBuild(HashSet<GOAP_Worldstate> goalWorldState, List<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState, GOAP_Agent agent)
    {

        Node current = null;

        HashSet<Node> closedSet = new HashSet<Node>();
        List<Node> openSet = new List<Node>();
        openSet.Add(new Node(null, goalWorldState, null, 0, true));
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
            if (current.isSkilled)
                plannerLog += makeIndent(graphDepth) + "-><color=#00CC00>ClosedSet Updated</color> (" + current.estimatedPathCost + "); ";
            else
                plannerLog += makeIndent(graphDepth) + "-><color=#0000CC>ClosedSet Updated</color> (" + current.estimatedPathCost + "); ";
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

                Node neighbor = GetValidNeighborNode(current, availableActions[i], currentWorldState, agent);
                if(neighbor != null && !openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                    msg = "";
                    foreach (GOAP_Worldstate state in neighbor.required)
                    {
                        msg += state.key.ToString() + ",";
                    }
                    if(neighbor.isSkilled)
                        plannerLog += makeIndent(graphDepth) + "-><color=#CCCC00>OpenSet Updated</color> (" + neighbor.estimatedPathCost + "); ";
                    else
                        plannerLog += makeIndent(graphDepth) + "-><color=#00CCCC>OpenSet Updated</color> (" + neighbor.estimatedPathCost + "); ";
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

    private Node GetValidNeighborNode(Node activeNode, GOAP_Action action, HashSet<GOAP_Worldstate> currentWorldState, GOAP_Agent agent)
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

        float skillModifier = 1f;
        bool isSkilled = true;
        if(action.RequiredSkill != null)
        {
            int index = agent.character.skills.IndexOf(action.RequiredSkill);
            if (index != -1)
            {
                int difference = action.RequiredSkill.level - agent.character.skills[index].level;
                if (difference > 0) skillModifier *= difference + 1;
                else skillModifier /= (-difference)+1;
            }
            else
            {
                isSkilled = false;
                skillModifier = 5f;
            }
        }

        return isValidAction ? new Node(activeNode, newRequired, action, newRequired.Count + action.ActionCost * skillModifier + activeNode.estimatedPathCost, isSkilled) : null;
    }

    private Queue<GOAP_Action> makeQueue(Node start, GOAP_Agent agent)
    {
        Queue<GOAP_Action> queue = new Queue<GOAP_Action>();
        string message = "<color=#00AA00>ActionQueue:</color> ";
        Node current = start;
        bool needsQuest = false;
        List<GOAP_Worldstate> questRequiredStates = new List<GOAP_Worldstate>();
        List<GOAP_Worldstate> questProvidedStates = new List<GOAP_Worldstate>();

        while (current.parent != null)
        {
            if(current.isSkilled)
            {
                queue.Enqueue(current.action);
                message += " -> " + current.action.ActionID;
                if(!needsQuest)
                {
                    foreach(GOAP_Worldstate state in current.action.SatisfyWorldStates)
                    {
                        questProvidedStates.Add(state);
                    }
                }
            }
            else
            {
                queue.Clear();
                message = " -> <color=#CC0000> QUEST instead of " + current.action.ActionID + "</color>";
                foreach (GOAP_Worldstate state in current.action.SatisfyWorldStates)
                {
                    questRequiredStates.Add(state);
                }
                needsQuest = true;
            }

            current = current.parent;
        }
        message += "|";
        Debug.Log(message);

        if (needsQuest)
        {
            GOAP_Quest quest = new GOAP_Quest(agent, questRequiredStates, questProvidedStates);
            agent.postedQuest = quest;
            GOAP_WhiteBoard.instance.AddQuest(quest);
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

