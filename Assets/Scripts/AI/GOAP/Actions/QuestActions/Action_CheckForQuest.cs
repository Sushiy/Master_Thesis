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
        workCost = 1f;
        actionID = "CheckForQuest";
        AddSatisfyWorldState(WorldStateKey.bHasWorked, 1);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        //Check if there is at least one unchecked Quest on the questboard
        chosenQuest = agent.CheckForQuests();
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
        
        //Try to plan for the chosen quest
        Queue<GOAP_Action> actionQueue = GOAP_Planner.instance.Plan(agent, new List<GOAP_Worldstate>(chosenQuest.RequiredStates), agent.currentWorldstates);
        agent.ResetPlanningTimer();
        
        if (actionQueue != null && actionQueue.Count > 0)
        {
            //if a valid plan was found, add it to the actionQueue 
            agent.activeQuest = chosenQuest;
            agent.UpdateActionQueue(actionQueue);
            return true;
        }

        chosenQuest = agent.CheckForQuests();

        return false;
    }
}
