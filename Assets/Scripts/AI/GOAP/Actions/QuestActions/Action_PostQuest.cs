﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_PostQuest : GOAP_Action
{
    public int questID
    {
        private set;
        get;        
    }
    QuestData questData;
    public Action_PostQuest()
    {
        Init();
        workCost = 8f;
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
            Debug.Log("<color=#0000cc><b>PERFORMING</b>: " + agent.Character.characterData.characterName + "</color>: PostQuest(" + questData.RequiredToString() + ")");
            agent.View.PrintMessage(ActionID);
        }

        UpdateWorkTime(deltaTime);
        
        if(completed)
        {
            GOAP_Quest q = new GOAP_Quest(questData);
            agent.postedQuestIDs.Add(q.id);
            questID = q.id;
            GOAP_QuestBoard.instance.AddQuest(q);
        }
        return completed;
    }

    public void AddQuestWorldstate(GOAP_Worldstate state)
    {
        questData.AddRequired(state);
    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}
