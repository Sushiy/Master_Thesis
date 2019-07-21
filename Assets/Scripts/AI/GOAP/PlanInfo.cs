using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanInfo
{
    private struct NodeInfo
    {
        public int id;

        public int parentid;
        public string requiredWorldstates;
        public string actionID;
        public float estimatedPathcost;

        public NodeInfo(int id, int parentid, string requiredWorldstates, string actionID, float estimatedPathcost)
        {
            this.id = id;
            this.parentid = parentid;
            this.requiredWorldstates = requiredWorldstates;
            this.actionID = actionID;
            this.estimatedPathcost = estimatedPathcost;
        }
    }

    public struct IterationInfo
    {
        public int iterationDepth;
        
        public string currentNode;
        public string openSet;
        public string chosenNode;

        public IterationInfo(int iterationDepth, string currentNode, string openSet, string chosenNode)
        {
            this.iterationDepth = iterationDepth;
            this.currentNode = currentNode;
            this.openSet = openSet;
            this.chosenNode = chosenNode;
        }
    }

    List<NodeInfo> nodes;
    public List<IterationInfo> iterations;

    public int PlanID { get; private set; }

    //includes states and targetQuest/personal/village
    public string goalInfo;
    public int questID = -1; //id of your own quest for this if it includes one

    public string characterName;

    public string actionQueueInfo;

    public PlanInfo(string goalInfo, string characterName)
    {
        this.goalInfo = goalInfo;
        this.characterName = characterName;
        nodes = new List<NodeInfo>();
        iterations = new List<IterationInfo>();
    }

    public void SetQuestID(int questID)
    {
        this.questID = questID;
    }

    //Only valid plans get an id
    public void ApprovePlan(int id, string actionQueueInfo)
    {
        PlanID = id;
        this.actionQueueInfo = actionQueueInfo;
    }

    public void AddNode(int nodeID, int parentID, string requiredWorldstates, string actionID, float estimatedPathcost)
    {
       nodes.Add(new NodeInfo(nodeID, parentID, requiredWorldstates, actionID, estimatedPathcost));
    }

    public void AddIteration(int iterationDepth, string currentNode, string openSet, string chosenNode)
    {
        iterations.Add(new IterationInfo(iterationDepth, currentNode, openSet, chosenNode));
    }
}
