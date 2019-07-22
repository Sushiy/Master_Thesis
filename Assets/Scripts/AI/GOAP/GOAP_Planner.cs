using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GOAP_Planner : MonoBehaviour
{
    public static GOAP_Planner instance;
    public float heuristicFactor = 2f;

    public bool writePlannerLog = true;
    string plannerLog = "";

    private Dictionary<string, Type> typeMap;

    private void AddToTypeMap(string className, Type type)
    {
        typeMap.Add(className, type);
    }

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        typeMap = new Dictionary<string, Type>();
        System.Type[] types = typeof(GOAP_Action).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GOAP_Action))).ToArray();
        for (int i = 0; i < types.Length; i++)
        {
            AddToTypeMap(types[i].ToString(), types[i]);
        }
    }

    public GOAP_Action InstantiateAction(string actionType)
    {
        try
        {
            Type t = typeMap[actionType];
            return (GOAP_Action)Activator.CreateInstance(t);
        }
        catch(KeyNotFoundException key)
        {
            Debug.LogError(actionType);
            return null;
        }
        catch(MissingMethodException)
        {
            return null;
        }
    }

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, List_GOAP_Worldstate goal, List_GOAP_Worldstate currentWorldState, List<string> availableActions)
    {
        plannerLog = "<color=#0000cc> <b>PLANNING</b>: " + agent.Character.characterName + "</color>\n";

        //Search for a valid plan
        Node startNode = WhileBuild(goal, availableActions, currentWorldState, agent);
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

    /// <summary>
    /// Perform an A*-search to find a valid plan
    /// </summary>
    /// <param name="goal">Agent's goal</param>
    /// <param name="availableActions">Agent's available actions</param>
    /// <param name="currentWorldState">Agent's current worldstate</param>
    /// <param name="agent">Agent that wants a plan</param>
    /// <returns>the starting node, if one was found, else null</returns>
    private Node WhileBuild(List_GOAP_Worldstate goal, List<string> availableActions, List_GOAP_Worldstate currentWorldState, GOAP_Agent agent)
    {
        PlanInfo planInfo = agent.planMemory[agent.planMemory.Count - 1];
        int currentNodeID = 0;

        Node current = null;

        List<Node> openSet = new List<Node>(); //This is a List so it can be sorted

        Node goalNode = GetGoalNode(currentWorldState, goal);
        goalNode.id = currentNodeID;
        planInfo.AddNode(goalNode.id, -1, goalNode.required.ToString(), "None", 0f);
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
            //Sort the open set by pathcosts and pick the best node
            openSet.Sort();
            string last = "";
            if (current!= null)
            {
                 last = current.ToString();
            }
            current = openSet[0];

            //Debug Log to visualize the process
            string openSetString = "";
            for (int i = 0; i < openSet.Count; i++)
            {
                openSetString += "\n- " + openSet[i].ToString();
            }
            if (writePlannerLog)
            {
                plannerLog += "\n Planning at depth" + graphDepth;
                
                plannerLog += "\n<color=#CCCC00>OpenSet(" + openSet.Count + "): </color>";
                plannerLog += openSetString;

                plannerLog += "\n\n";

                plannerLog += makeIndent(graphDepth) + "-><color=#00CC00>Best Node Chosen:</color> " + current.ToString() + "\n";
            }

            planInfo.AddIteration(graphDepth, "Current:\n- " + last.ToString(), "OpenSet(" + openSet.Count + "):" + openSetString, current.ToString());

            if (current.required.Count == 0)
            {
                plannerLog += makeIndent(graphDepth) + "-><color=#00CC00>Planning Completed with:</color> " + current.ToString() + "\n";
                return current;
            }

            openSet.Remove(current);

            bool foundValidNeighbor = false;
            for (int i = 0; i < availableActions.Count; i++)
            {
                if (availableActions[i].Equals("Action_" + current.action)) continue; //Dont do the same action twice

                Node neighbor = (availableActions[i] == "Action_BuyItem") ? GenerateBuyNode(current, currentWorldState,agent) : GetValidNeighborNode(current, InstantiateAction(availableActions[i]), currentWorldState, agent);
                if(neighbor != null)
                {
                    neighbor.id = currentNodeID++;
                    foundValidNeighbor = true;
                    int indexOfNodeWithSameState = openSet.IndexOf(neighbor);
                    if (indexOfNodeWithSameState != -1)
                    {
                        string tmp = openSet[indexOfNodeWithSameState].ToString();
                        //if there is already another node with the same resulting planningworldstate, check which has the lower pathcost
                        if (openSet[indexOfNodeWithSameState].estimatedPathCost > neighbor.estimatedPathCost)
                        {
                            //if the new node has a lower pathcost, pick that
                            openSet.Remove(openSet[indexOfNodeWithSameState]);                            
                            openSet.Add(neighbor);

                            if (writePlannerLog)
                            {
                                plannerLog += makeIndent(graphDepth) + "-><color=#CCCC00>OpenSet Replaced</color>  " + tmp + "with" + neighbor.ToString() + "\n";
                            }
                        }
                        else
                        {
                            //if the new node has a higher pathcost, don't do anything
                            if (writePlannerLog)
                            {
                                plannerLog += makeIndent(graphDepth) + "-><color=#CC0000>OpenSet Not Replaced</color>  " + tmp + "with" + neighbor.ToString() + "\n";
                            }
                            neighbor = null;
                        }
                    }
                    else
                    {
                        openSet.Add(neighbor);
                        if(neighbor.action.ActionID == "GatherWood")
                        {
                            int a = 0;
                            a += 1;
                        }
                        //Debug Log to visualize the process
                        if (writePlannerLog)
                        {
                            plannerLog += makeIndent(graphDepth) + "-><color=#CCCC00>OpenSet Updated</color>  " + neighbor.ToString() + "\n";
                        }
                    }
                }
            }
            if(!foundValidNeighbor && current != goalNode)
            {
                plannerLog += "No valid neigbor found:\n";
                Node questNode = GenerateQuestNode(current, currentWorldState, agent);
                questNode.id = currentNodeID++;
                openSet.Add(questNode);
                //Debug Log to visualize the process
                if (writePlannerLog)
                {
                    plannerLog += makeIndent(graphDepth) + "-><color=#660000>OpenSet Updated</color>  " + questNode.ToString() + "\n";
                }
            }
            graphDepth++;

        }

        return null;
    }

    /// <summary>
    /// Checks if the goal worldstate is already satisfied within the current worldstate
    /// </summary>
    /// <param name="currentWorldState"></param>
    /// <param name="goalWorldState"></param>
    /// <returns></returns>
    public bool IsGoalSatisfied(List_GOAP_Worldstate currentWorldState, GOAP_Worldstate goalWorldState)
    {
        //First, check if we have not already reached the goal, by checking it against our currentWorldstate
        
        if (!currentWorldState.ContainsExactly(goalWorldState))
            return false;
        return true;
    }
    
    /// <summary>
    /// Combines current and goal worldstate
    /// </summary>
    /// <param name="currentWorldState"></param>
    /// <param name="goalWorldState"></param>
    /// <returns></returns>
    private Node GetGoalNode(List_GOAP_Worldstate currentWorldState, List_GOAP_Worldstate goalWorldState)
    {
        List_GOAP_Worldstate newRequired = new List_GOAP_Worldstate(goalWorldState);
        plannerLog += "Found Goal:\n";
        for (int state = 0; state < goalWorldState.Count; state++)
        {
            plannerLog += goalWorldState[state].ToString() + "\n";
            if(currentWorldState.ContainsExactly(goalWorldState[state]))
            { 
                newRequired.Remove(goalWorldState[state]);
            }
        }
        plannerLog += "\n";
        return new Node(null, newRequired, null, 0);
    }
    
    /// <summary>
    /// Tries to apply the action onto the activeNode to see if it results in a valid neighbor
    /// </summary>
    /// <param name="activeNode"></param>
    /// <param name="action"></param>
    /// <param name="planningWorldState">worldstate at the current stage of planning</param>
    /// <param name="agent">currently planning agent</param>
    /// <returns></returns>
    private Node GetValidNeighborNode(Node activeNode, GOAP_Action action, List_GOAP_Worldstate planningWorldState, GOAP_Agent agent)
    {
        if (action == null) return null;
        bool isUsefulAction = false;

        List_GOAP_Worldstate newRequired = new List_GOAP_Worldstate(activeNode.required);

        //Actions need to fulfill at least one required Worldstate to result in a valid neighbor
        foreach (GOAP_Worldstate state in activeNode.required)
        {
            if (action.SatisfyWorldstates.ContainsExactly(state))
            {
                newRequired.Remove(state);
                isUsefulAction = true;
            }
        }

        //if this action does not help the plan, return null
        if (!isUsefulAction) return null;
        //If the actions proceduralConditions are not met, we can't perform it anyways
        //if (!action.CheckProceduralConditions(agent)) return null;

        //add the actions own required worldstates to the Node
        foreach (GOAP_Worldstate state in action.RequiredWorldstates)
        {
            if (!planningWorldState.ContainsExactly(state))
            {
                newRequired.Add(state);
            }
        }

        //Apply skillmodification onto the neighbor if it is valid
        float skillModifier = 1f;
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
                skillModifier = 5f;
            }
        }

        //Change the skillmodifier on the action 
        //action.ApplySkillModifier(skillModifier);

        return new Node(activeNode, newRequired, action, newRequired.Count * heuristicFactor + action.ActionCost + activeNode.estimatedPathCost);
    }

    private Node GenerateBuyNode(Node activeNode, List_GOAP_Worldstate planningWorldState, GOAP_Agent agent)
    {
        List_GOAP_Worldstate newRequired = new List_GOAP_Worldstate(activeNode.required);
        Action_BuyItem action = new Action_BuyItem();

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
        return new Node(activeNode, newRequired, action, estimatedBuyCost + activeNode.estimatedPathCost);
    }
    
    /// <summary>
    /// Generates a Node containing a PostQuest action from the activeNodes required worldstates
    /// </summary>
    /// <param name="activeNode"></param>
    /// <param name="planningWorldState"></param>
    /// <param name="agent"></param>
    /// <returns></returns>
    private Node GenerateQuestNode(Node activeNode, List_GOAP_Worldstate planningWorldState, GOAP_Agent agent)
    {
        List_GOAP_Worldstate newRequired = new List_GOAP_Worldstate();
        Action_PostQuest action = new Action_PostQuest();
        foreach(GOAP_Worldstate state in activeNode.required)
        {
            //Debug.Log("Adding state " + state.ToString() + " to quest");
            action.AddQuestWorldstate(state);
        }
        float estimatedQuestCost = action.ActionCost * activeNode.required.Count;
        return new Node(activeNode, newRequired, action, estimatedQuestCost + activeNode.estimatedPathCost);
    }
    
    /// <summary>
    /// Forms a queue of actions from the nodes generated by the A*-search
    /// </summary>
    /// <param name="start">The first node</param>
    /// <param name="agent"></param>
    /// <returns></returns>
    private Queue<GOAP_Action> MakeQueue(Node start, GOAP_Agent agent)
    {
        Queue<GOAP_Action> queue = new Queue<GOAP_Action>();
        string message = "<color=#00AA00>ActionQueue:</color> ";
        Node current = start;

        while (current.parent != null)
        {
            queue.Enqueue(current.action);
            message += " -> " + current.action.ActionID;
            if (current.action.ActionID == "PostQuest")
            {
                Action_WaitForQuest waitForQuest = new Action_WaitForQuest();
                queue.Enqueue(waitForQuest);
                message += " -> " + waitForQuest.ActionID;
            }
            current = current.parent;
        }

        if(agent.activeQuest != null)
        {
            Action_CompleteQuest completeQuest = new Action_CompleteQuest(agent.activeQuest.id);
            completeQuest.SetActionTarget(agent.activeQuest.Owner);
            queue.Enqueue(completeQuest);
            message += " -> " + completeQuest.ActionID;
        }

        message += "|";
        plannerLog += message;
        Debug.Log(plannerLog);
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
        public int id;
        public Node parent;
        public float estimatedPathCost;
        public List_GOAP_Worldstate required;
        public GOAP_Action action;

        public Node(Node parent, List_GOAP_Worldstate required, GOAP_Action action, float estimatedPathCost)
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

        public bool Equals(Node other)
        {
            //Equalize through states only

            //First check the number of states
            if (required.Count != other.required.Count) return false;

            //Then check if the other state contains all the same states this one does
            bool sameRequiredStates = true;
            foreach(GOAP_Worldstate state in required)
            {
                if(!other.required.ContainsExactly(state))
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

        public override string ToString()
        {
            string msg = "Node" + id;
            msg += "(Required(" + required.Count + "):";
            if(required.Count < 0)
            {
                msg += "empty";
            }
            else
            {
                msg += required.ToString();
            }
            msg += "; Action:" + (action!= null? action.ActionID : "none") + ";Cost:" + estimatedPathCost + ")";
            return msg;
        }
    }
}

