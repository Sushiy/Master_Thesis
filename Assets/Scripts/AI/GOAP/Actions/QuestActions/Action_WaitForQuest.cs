using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Action_WaitForQuest : GOAP_Action
{
    int ownQuestID = -1;
    int originalQuestID = -1;
    public Action_WaitForQuest()
    {
        Init();
        workCost = 0f;
        actionID = "WaitForQuest";
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool Perform(GOAP_Agent agent, float deltaTime)
    {
        if(isStartingWork)
        {
            if(ownQuestID < 0)
            {
                ownQuestID = agent.postedQuestIDs.Last();
                agent.View.PrintMessage(ActionID);
                Debug.Log("<color=#0000cc><b>PERFORMING</b>: " + agent.Character.characterData.characterName + "</color>: WaitForQuest " + ownQuestID);

                //Move this plan to questPlans.
                agent.planMemory[(int)agent.activePlanInfo].SetQuestID(ownQuestID);
                agent.SaveQuestPlan(ownQuestID);
                if(agent.activeQuest != null)
                    originalQuestID = agent.activeQuest.id;
                return true;
            }
            else
            {
                Debug.Log("<color=#0000cc><b>Restarting</b>: " + agent.Character.characterData.characterName + "</color>: WaitForQuest " + ownQuestID);
                UpdateWorkTime(deltaTime);
                if (originalQuestID != -1)
                {
                    agent.activeQuest = GOAP_QuestBoard.instance.quests[originalQuestID];
                    agent.activeGoal.Clear();
                    agent.activeGoal.AddRange(agent.activeQuest.RequiredStates);
                }
                agent.activePlanInfo = agent.planMemory.FindIndex(x => x.questID == ownQuestID);
                if (agent.activePlanInfo == -1)
                {
                    agent.activePlanInfo = null;
                    Debug.LogError(agent.Character.characterData.characterName + " ActivePlanIndex: -1");
                }
            }
        }

        if(agent.completedQuestIDs.Contains(ownQuestID))
        {
            //Finish Quest
            if(originalQuestID == -1 || GOAP_QuestBoard.instance.quests.Keys.Contains(originalQuestID))
            {
                Debug.Log("<color=#0000cc>" + agent.Character.characterData.characterName + "s</color> Quest " + ownQuestID + " was completed!" + ((originalQuestID > -1)? "\nThis plan is a solution for Quest " + originalQuestID : ""));
                agent.completedQuestIDs.Remove(ownQuestID);
                foreach (GOAP_Worldstate state in SatisfyWorldstates)
                {
                    agent.ChangeCurrentWorldState(state);
                }
                return true;
            }
            else
            {
                Debug.Log("<color=#0000cc><b>Canceling</b>: " + agent.Character.characterData.characterName + "</color>: WaitForQuest " + ownQuestID);
                agent.CancelPlan();
                return true;
            }
        }

        return false;
    }

    public void AddQuestWorldstate(GOAP_Worldstate state)
    {
        SatisfyWorldstates.Add(state);
    }
}
