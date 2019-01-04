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
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        quest = new GOAP_Quest(agent);
        return true;
    }

    public override bool RequiresInRange()
    {
        target = InfoBlackBoard.instance.questBoardLocation; //TODO: This isnt technically correct
        if (target != null)
            return true;
        else
            return false;
    }

    public override bool Perform(GOAP_Agent agent)
    {
        Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is performing: " + actionID);
        agent.postedQuest = quest;
        agent.View.PrintMessage(ActionID, workCost);
        GOAP_QuestBoard.instance.AddQuest(quest);
        return true;
    }

    public void AddQuestWorldstate(GOAP_Worldstate state)
    {
        quest.AddRequired(state);
    }
}
