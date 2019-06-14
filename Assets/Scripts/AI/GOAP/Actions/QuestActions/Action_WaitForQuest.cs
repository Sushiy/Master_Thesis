using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Action_WaitForQuest : GOAP_Action
{
    int questID;
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
            Debug.Log("<color=#0000cc><b>PERFORMING</b>: " + agent.Character.characterName + "</color>: WaitForQuest");
            agent.View.PrintMessage(ActionID);
            UpdateWorkTime(deltaTime);
            questID = agent.postedQuestIDs.Last();

            //Move this plan to questPlans.
            agent.SaveQuestPlan(questID);
        }

        if(agent.completedQuestIDs.Contains(questID))
        {
            //Finish Quest
            Debug.Log("<color=#0000cc>" + agent.Character.characterName + "s</color> Quest was completed!");
            foreach (GOAP_Worldstate state in SatisfyWorldstates)
            {
                agent.ChangeCurrentWorldState(state);
            }
            return true;
        }

        return false;
    }

    public void AddQuestWorldstate(GOAP_Worldstate state)
    {
        SatisfyWorldstates.Add(state);
    }
}
