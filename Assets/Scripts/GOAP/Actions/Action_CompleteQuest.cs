using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CompleteQuest : GOAP_Action
{
    bool hasLogged = false;
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

    public override bool Perform(GOAP_Agent agent)
    {
        Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> completed Quest " + agent.activeQuest.id);
        
        TradeQuestItems(agent);

        agent.activeQuest.Complete();
        agent.activeQuest = null;
        return true;
    }

    public void TradeQuestItems(GOAP_Agent agent)
    {
        foreach(GOAP_Worldstate questState in agent.activeQuest.RequiredStates)
        {
            if(questState.key == WorldStateKey.eHasItem)
            {
                otherAgent.Character.UpdateInventory((ItemIds)questState.value, true);
                agent.Character.UpdateInventory((ItemIds)questState.value, false);
            }
        }
    }

    public void SetActionTarget(GOAP_Agent otherAgent)
    {
        this.otherAgent = otherAgent;
        target = otherAgent.View.GetActionTargetSelf();
    }
}
