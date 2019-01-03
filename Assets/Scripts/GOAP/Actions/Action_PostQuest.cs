using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_PostQuest : GOAP_Action
{
    GOAP_Quest quest;
    public Action_PostQuest()
    {
        Init();
        workCost = 10f;
        actionID = "PostQuest";
        keepOpen = true;
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        quest = new GOAP_Quest(agent);
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool Run(GOAP_Agent agent)
    {
        agent.activeQuest = quest;
        GOAP_QuestBoard.instance.AddQuest(quest);
        return true;
    }

    public void AddQuestWorldstate(GOAP_Worldstate state)
    {
        quest.AddRequired(state);
    }
}
