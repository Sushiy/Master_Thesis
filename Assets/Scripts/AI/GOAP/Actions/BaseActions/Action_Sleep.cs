using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Sleep : GOAP_Action 
{

	public Action_Sleep()
    {
        Init();
        actionID = "Sleep";
        workCost = 10f;
        AddSatisfyWorldState(WorldStateKey.bHasSlept, true);
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
        if (completed)
        {
            agent.Character.Sleep();
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}
