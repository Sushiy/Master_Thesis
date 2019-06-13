using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_PostQuest : GOAP_Action
{
    QuestData questData;
    public Action_PostQuest()
    {
        Init();
        workCost = 30f;
        actionID = "PostQuest";
        questData.Init();
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        if (questData.owner != agent) questData.owner = agent;
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
            Debug.Log("<color=#0000cc><b>PERFORMING</b>: " + agent.Character.characterName + "</color>: PostQuest(" + questData.RequiredToString() + ")");
            agent.View.PrintMessage(ActionID);
        }

        UpdateWorkTime(deltaTime);
        
        if(completed)
        {
            agent.postedQuest = new GOAP_Quest(questData);
            GOAP_QuestBoard.instance.AddQuest(agent.postedQuest);
        }
        return completed;
    }

    public void AddQuestWorldstate(GOAP_Worldstate state)
    {
        questData.AddRequired(state);
    }
}
