using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CompleteQuest : GOAP_Action
{
    int questID = -1;
    GOAP_Agent otherAgent;
    public Action_CompleteQuest(int questID)
    {
        Init();
        workCost = 1f;
        actionID = "CompleteQuest";
        this.questID = questID;
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
            Debug.Log("<color=#0000cc>" + agent.Character.characterData.characterName + "</color> is completing Quest " + questID);
        }
        if(!GOAP_QuestBoard.instance.quests.ContainsKey(questID))
        {
            Debug.Log("<color=#0000cc>" + agent.Character.characterData.characterName + "</color> can't complete quest, already finished");
            agent.CancelPlan();
            return true;
        }

        UpdateWorkTime(deltaTime);
        if(completed)
        {
            TradeQuestItems(agent);
            otherAgent.ReceiveQuestCompletion(questID);
            agent.activeQuest = null;
        }

        return completed;
    }

    public void TradeQuestItems(GOAP_Agent agent)
    {
        GOAP_Quest q = GOAP_QuestBoard.instance.quests[questID];
        foreach(GOAP_Worldstate questState in q.RequiredStates)
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
