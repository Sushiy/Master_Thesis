using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Sleep : GOAP_Action 
{

	public Action_Sleep()
    {
        Init();
        actionID = "Sleep";
        workCost = 60f;
        AddSatisfyWorldState(WorldStateKey.bIsTired, false);
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
        StartPerform(agent);
        UpdateWorkTime(deltaTime);
        agent.View.VisualizeAction(this);
        if (completed)
        {
            agent.Character.Sleep();
            CompletePerform(agent);
        }
        return completed;
    }
}
