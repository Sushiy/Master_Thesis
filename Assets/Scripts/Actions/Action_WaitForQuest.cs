using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_WaitForQuest : GOAP_Action
{
    public Action_WaitForQuest()
    {
        Init();
        workCost = 0f;
        actionID = "WaitForQuest";
        keepOpen = true;
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool Run(GOAP_Agent agent)
    {
        if (agent.postedQuest != null) return false;
        //Debug.Log("Quest was completed");
        return true;
    }

    public void AddQuestWorldstate(GOAP_Worldstate state)
    {
        SatisfyWorldstates.Add(state);
    }
}
