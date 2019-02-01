using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CompleteQuest : GOAP_Action
{
    GOAP_Agent otherAgent;
    public Action_CompleteQuest()
    {
        Init();
        workCost = 1f;
        actionID = "CompleteQuest";
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool Perform(GOAP_Agent agent, float deltaTime)
    {
        if(isStartingWork)
        {
            Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is completing Quest " + agent.activeQuest.id);
        }

        UpdateWorkTime(deltaTime);
        if(completed)
        {
            TradeQuestItems(agent);

            agent.activeQuest.Complete();
            agent.activeQuest = null;
        }

        return completed;
    }

    public void TradeQuestItems(GOAP_Agent agent)
    {
        foreach(GOAP_Worldstate questState in agent.activeQuest.RequiredStates)
        {
            if(questState.key == WorldStateKey.eHasItem)
            {
                otherAgent.Character.UpdateInventory((ItemType)questState.value, true);
                agent.Character.UpdateInventory((ItemType)questState.value, false);
            }
        }
    }

    public void SetActionTarget(GOAP_Agent otherAgent)
    {
        this.otherAgent = otherAgent;
        target = otherAgent.View.GetActionTargetSelf();
    }
}
