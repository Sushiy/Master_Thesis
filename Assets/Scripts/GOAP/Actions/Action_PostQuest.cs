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
        if (quest == null) quest = new GOAP_Quest(agent);
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

    public override bool Perform(GOAP_Agent agent, float deltaTime)
    {
        if (isStartingWork)
        {
            Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is performing: " + actionID);
            agent.View.PrintMessage(ActionID);
        }

        UpdateWorkTime(deltaTime);
        
        if(completed)
        {
            agent.postedQuest = quest;
            GOAP_QuestBoard.instance.AddQuest(quest);
        }
        return completed;
    }

    public void AddQuestWorldstate(GOAP_Worldstate state)
    {
        quest.AddRequired(state);
    }
}
