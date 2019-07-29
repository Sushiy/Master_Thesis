using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class GOAP_Planner
{
    public static float heuristicFactor = 2f;

    public static bool writePlannerLog = true;
    static string plannerLog = "";

    private static Dictionary<string, Type> typeMap;

    public static void Init()
    {
        typeMap = new Dictionary<string, Type>();
        System.Type[] types = typeof(GOAP_Action).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GOAP_Action))).ToArray();
        for (int i = 0; i < types.Length; i++)
        {
            AddToTypeMap(types[i].ToString(), types[i]);
        }
    }

    public static GOAP_Action InstantiateAction(string actionType)
    {
        try
        {
            Type t = typeMap[actionType];
            return (GOAP_Action)Activator.CreateInstance(t);
        }
        catch(KeyNotFoundException)
        {
            Debug.LogError(actionType);
            return null;
        }
        catch(MissingMethodException)
        {
            return null;
        }
    }
    /// <summary>
    /// Initiate a new plan
    /// </summary>
    /// <param name="agent">Agent that is planning</param>
    /// <param name="goal">Agent's goal</param>
    /// <param name="currentWorldState">Agent's current worldstate</param>
    /// <param name="availableActions">Agent's available actions</param>
    /// <returns></returns>
    public static Queue<GOAP_Action> Plan(GOAP_Agent agent, List_GOAP_Worldstate goal, List_GOAP_Worldstate currentWorldState, List<string> availableActions)
    {
        plannerLog = "<color=#0000cc> <b>PLANNING</b>: " + agent.Character.characterData.characterName + "</color>\n";

        //Search for a valid plan
        Node startNode = WhileBuild(goal, availableActions, currentWorldState, agent);
        plannerLog += "\n";

        //Return null if you couldn't find a plan!
        if (startNode == null)
        {
            plannerLog += "<color=#cc0000>Couldn't find actions fulfilling " + agent.Character.characterData.characterName + "s goal.</color>\n";

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
        plannerLog += "<color=#00cc00>" + agent.Character.characterData.characterName + "found plan:</color>\n";

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
    private static Node WhileBuild(List_GOAP_Worldstate goal, List<string> availableActions, List_GOAP_Worldstate currentWorldState, GOAP_Agent agent)
    {
        //Pick the latest planInfo
        PlanInfo planInfo = agent.planMemory[agent.planMemory.Count - 1];
        int currentNodeID = 0;

        Node current = null;

        List<Node> openSet = new List<Node>();

        //Get a Node for the goal
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
            if(graphDepth >= 20)
            {
                Debug.LogError("Plan must be recursive and has exceeded 20 iterations");
                break;
            }
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
                openSetString += "\n" + makeIndent(graphDepth) + "- " + openSet[i].ToString();
            }
            if (writePlannerLog)
            {
                plannerLog += "\n" + makeIndent(graphDepth) + "Planning at depth" + graphDepth;
                plannerLog += "\n" + makeIndent(graphDepth) + "Current: " + last;
                
                plannerLog += "\n" + makeIndent(graphDepth) + "<color=#CCCC00>OpenSet(" + openSet.Count + "): </color>";
                plannerLog += openSetString;

                plannerLog += "\n";

                plannerLog += makeIndent(graphDepth) + "-><color=#00CC00>Best Node Chosen:</color> " + current.ToString() + "\n\n";
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

                List<Node> neighbors = GetValidNeighborNodeVariations(current, InstantiateAction(availableActions[i]), currentWorldState, agent);
                if(neighbors != null && neighbors.Count > 0)
                {
                    for (int j = 0; j < neighbors.Count; j++)
                    {
                        if (neighbors[j] == null) break;
                        foundValidNeighbor = true;
                        neighbors[j].id = currentNodeID++;
                        int indexOfNodeWithSameState = openSet.IndexOf(neighbors[j]);
                        if (indexOfNodeWithSameState != -1)
                        {
                            string tmp = openSet[indexOfNodeWithSameState].ToString();
                            //if there is already another node with the same resulting planningworldstate, check which has the lower pathcost
                            if (openSet[indexOfNodeWithSameState].estimatedPathCost > neighbors[j].estimatedPathCost)
                            {
                                //if the new node has a lower pathcost, pick that
                                openSet.Remove(openSet[indexOfNodeWithSameState]);
                                openSet.Add(neighbors[j]);

                                if (writePlannerLog)
                                {
                                    plannerLog += makeIndent(graphDepth) + "-><color=#CCCC00>OpenSet Replaced</color>  " + tmp + "with" + neighbors[j].ToString() + "\n";
                                }
                            }
                            else
                            {
                                //if the new node has a higher pathcost, don't do anything
                                if (writePlannerLog)
                                {
                                    plannerLog += makeIndent(graphDepth) + "-><color=#CC0000>OpenSet Not Replaced</color>  " + tmp + "with" + neighbors[j].ToString() + "\n";
                                }
                            }
                        }
                        else
                        {
                            openSet.Add(neighbors[j]);
                            //Debug Log to visualize the process
                            if (writePlannerLog)
                            {
                                plannerLog += makeIndent(graphDepth) + "-><color=#CCCC00>OpenSet Updated</color>  " + neighbors[j].ToString() + "\n";
                            }
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
    public static bool IsGoalSatisfied(List_GOAP_Worldstate currentWorldState, GOAP_Worldstate goalWorldState)
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
    private static Node GetGoalNode(List_GOAP_Worldstate currentWorldState, List_GOAP_Worldstate goalWorldState)
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
    private static List<Node> GetValidNeighborNodeVariations(Node activeNode, GOAP_Action action, List_GOAP_Worldstate planningWorldState, GOAP_Agent agent)
    {
        if (action == null) return null;

        List<Node> nodes = new List<Node>();

        if(action.HasVariations())
        {
            for(int i = 0; i < action.variations.Count; i++)
            {
                if(action.variations[i] != null)
                {
                    nodes.Add(GetValidNeighborNode(activeNode, action.GetVariation(i), planningWorldState, agent));
                }
            }
        }
        else
        {
            nodes.Add(GetValidNeighborNode(activeNode, action, planningWorldState, agent));
        }

        return nodes;
    }
    
    private static Node GetValidNeighborNode(Node activeNode, GOAP_Action action, List_GOAP_Worldstate planningWorldState, GOAP_Agent agent)
    {
        bool isUsefulAction = false;

        List_GOAP_Worldstate newRequired = new List_GOAP_Worldstate(activeNode.required);

        //Actions need to fulfill at least one required Worldstate to result in a valid neighbor
        foreach (GOAP_Worldstate state in activeNode.required)
        {
            if (action.SatisfyWorldstates.ContainsExactly(state))
            {
                if (state.key == WorldStateKey.bWasFieldTended && state.value == 0)
                {
                    Debug.LogError(action.SatisfyWorldstates.ToString());
                }
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
                //If the state is an observable one and the agent does not have any memory of it, they just assume that it is in their favor
                if (state.IsObservableState && !agent.currentWorldstates.ContainsKey(state))
                {
                    Debug.Log("<color=#cc00cc>" + agent.Character.characterData.characterName + "</color> assumes state:" + state.ToString());
                    agent.ChangeCurrentWorldState(state);
                }
                else
                {
                    newRequired.Add(state);
                }
            }
        }

        //Apply skillmodification onto the neighbor if it is valid
        float skillModifier = 1f;
        if (action.BenefitingSkill != Skills.None)
        {
            GOAP_Skill skill = agent.Character.characterData.skills.Find(x => x.id == action.BenefitingSkill);
            if (skill != null)
            {
                //If the character is actually skilled in this action, adjust the skillmodifier
                skillModifier /= skill.level;
            }
            else
            {
                //If the character is not skilled in this action, the skillmodifier is set to 5. This only comes into play, when global knowledge planning is used.
                skillModifier = 1f;
            }
        }

        //Change the skillmodifier on the action 
        action.ApplySkillModifier(skillModifier);

        return new Node(activeNode, newRequired, action, newRequired.Count * heuristicFactor + action.ActionCost + activeNode.estimatedPathCost);
    }

    /// <summary>
    /// Generates a Node containing a PostQuest action from the activeNodes required worldstates
    /// </summary>
    /// <param name="activeNode"></param>
    /// <param name="planningWorldState"></param>
    /// <param name="agent"></param>
    /// <returns></returns>
    private static Node GenerateQuestNode(Node activeNode, List_GOAP_Worldstate planningWorldState, GOAP_Agent agent)
    {
        List_GOAP_Worldstate newRequired = new List_GOAP_Worldstate();
        Action_PostQuest action = new Action_PostQuest();
        foreach(GOAP_Worldstate state in activeNode.required)
        {
            //Debug.Log("Adding state " + state.ToString() + " to quest");
            action.AddQuestWorldstate(state);
        }
        float estimatedQuestCost = action.ActionCost * activeNode.required.Count * heuristicFactor;
        return new Node(activeNode, newRequired, action, estimatedQuestCost + activeNode.estimatedPathCost);
    }
    
    /// <summary>
    /// Forms a queue of actions from the nodes generated by the A*-search
    /// </summary>
    /// <param name="start">The first node</param>
    /// <param name="agent"></param>
    /// <returns></returns>
    private static Queue<GOAP_Action> MakeQueue(Node start, GOAP_Agent agent)
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
        agent.Character.Log(plannerLog);
        return queue;
    }

    private static string makeIndent(int depth)
    {
        string s = "";
        for(int i = 0; i<depth; i++)
        {
            s += "  ";
        }
        return s;
    }

    private static void AddToTypeMap(string className, Type type)
    {
        typeMap.Add(className, type);
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

