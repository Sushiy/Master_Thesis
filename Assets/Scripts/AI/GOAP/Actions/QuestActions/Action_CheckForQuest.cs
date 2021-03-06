using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CheckForQuest : GOAP_Action
{
    GOAP_Agent otherAgent;

    public Action_CheckForQuest()
    {
        Init();
        workCost = 5f;
        actionID = "CheckForQuest";
        AddSatisfyWorldState(WorldStateKey.bHasCheckedQuestboard, 1);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        if (target == null)
            target = InfoBlackBoard.instance.questBoardLocation;
        return true;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool Perform(GOAP_Agent agent, float deltaTime)
    {
        StartPerform(agent);
        alphaWorkTime = 0.5f;

        //Add a new plan for this quest.
        agent.activeQuest = agent.CheckForQuests();

        if(agent.activeQuest != null)
        {
            //if there is a quest, set the agents goal and add a planInfo
            agent.activeGoal = agent.activeQuest.RequiredStates;
            agent.planMemory.Add(new PlanInfo(agent.PrintGoal(), agent.Character.characterData.characterName));

            Queue<GOAP_Action> actionQueue = GOAP_Planner.Plan(agent, new List_GOAP_Worldstate(agent.activeQuest.RequiredStates), agent.currentWorldstates, agent.Character.characterData.availableActions);
            agent.ResetPlanningTimer();

            if (actionQueue != null && actionQueue.Count > 0)
            {
                //Approve the planInfo
                agent.planMemory[agent.planMemory.Count - 1].ApprovePlan(agent.planMemory.Count - 1, agent.PrintActionQueue());
                agent.activePlanInfo = agent.planMemory.Count - 1;

                //if a valid plan was found, add it to the actionQueue
                agent.Character.Log("<color=#0000cc>" + agent.Character.characterData.characterName + "</color> found a valid Plan for Quest " + agent.activeQuest.id);
                agent.UpdateActionQueue(actionQueue);
                agent.checkedCharacterGoals.Remove(new GOAP_Worldstate(WorldStateKey.bHasCheckedQuestboard, 1));
                CompletePerform(agent);
                return true;
            }
            else
            {
                agent.planMemory.RemoveAt(agent.planMemory.Count - 1);
                agent.Character.Log("<color=#0000cc>" + agent.Character.characterData.characterName + "</color> didn't find a valid Plan for Quest " + agent.activeQuest.id);
                agent.activeQuest = null;
                agent.activeGoal = null;
                return false;
            }
        }
        else
        {
            agent.Character.Log("<color=#0000cc>" + agent.Character.characterData.characterName + "</color> didn't find any Quests.");
            CompletePerform(agent);
            agent.checkedCharacterGoals.Remove(new GOAP_Worldstate(WorldStateKey.bHasCheckedQuestboard, 1));
            return true;
        }

    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}
