using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_GetWater : GOAP_Action 
{

	public Action_GetWater()
    {
        Init();
        actionID = "GetWater";
        workCost = 8f;
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Water);
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
            CompletePerform(agent);
        }
        return completed;
    }
}
