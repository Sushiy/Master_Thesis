using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CheckForQuest : GOAP_Action
{
    GOAP_Agent otherAgent;
    GOAP_Quest chosenQuest;

    public Action_CheckForQuest()
    {
        Init();
        workCost = 5f;
        actionID = "CheckForQuest";
        AddSatisfyWorldState(WorldStateKey.bHasCheckedQuestboard, 1);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        //Check if there is at least one unchecked Quest on the questboard
        if(chosenQuest == null)
            chosenQuest = agent.CheckForQuests();
        if(chosenQuest == null)
            Debug.Log("No quests available");
        if(target == null)
            target = InfoBlackBoard.instance.questBoardLocation;
        return (chosenQuest != null);
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool Perform(GOAP_Agent agent, float deltaTime)
    {
        StartPerform(agent);
        alphaWorkTime = 0.5f;

        if (!agent.AllowedToPlan) return false;
        Debug.Log("Performing CheckQuest");
        //Try to plan for the chosen quest
        agent.activeQuest = chosenQuest;
        Queue<GOAP_Action> actionQueue = GOAP_Planner.instance.Plan(agent, new List_GOAP_Worldstate(chosenQuest.RequiredStates), agent.currentWorldstates);
        agent.ResetPlanningTimer();
        
        if (actionQueue != null && actionQueue.Count > 0)
        {
            //if a valid plan was found, add it to the actionQueue
            Debug.Log("Found a valid Plan for Quest " + chosenQuest.id);
            agent.UpdateActionQueue(actionQueue);
            return true;
        }
        else
        {
            Debug.Log("Didn't find a valid Plan for Quest " + chosenQuest.id);
            agent.activeQuest = null;
        }

        chosenQuest = agent.CheckForQuests();

        return false;
    }
}
